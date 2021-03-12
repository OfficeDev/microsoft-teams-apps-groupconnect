// <copyright file="CardHelper.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using AdaptiveCards;
    using AdaptiveCards.Templating;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Azure.CognitiveServices.Knowledge.QnAMaker.Models;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.DIConnect.Common;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.EmployeeResourceGroup;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.UserPairupMapping;
    using Microsoft.Teams.Apps.DIConnect.Common.Resources;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.CommonBot;
    using Microsoft.Teams.Apps.DIConnect.Models;
    using Microsoft.Teams.Apps.DIConnect.Models.CardSetting;
    using Newtonsoft.Json;

    /// <summary>
    /// Class that handles adaptive card helper methods.
    /// </summary>
    public class CardHelper
    {
        /// <summary>
        /// Represents maximum prompt question count.
        /// </summary>
        private const int MaximumPromptQuestionCount = 4;

        /// <summary>
        /// Redirection url of tab.
        /// </summary>
        private const string TabRedirectionUrl = "https://teams.microsoft.com/l/entity";

        /// <summary>
        /// Entity id of the tab.
        /// </summary>
        private const string TabEntityId = "Discover";

        /// <summary>
        /// Represents the welcome card file name.
        /// </summary>
        private const string WelcomeCardFileName = "WelcomeCard.json";

        /// <summary>
        /// Represents the QnA response card file name.
        /// </summary>
        private const string QnAResponseCardFileName = "QnAResponseCard.json";

        /// <summary>
        /// Represents the share feedback card file name.
        /// </summary>
        private const string ShareFeedbackCardFileName = "ShareFeedbackCard.json";

        /// <summary>
        /// Represents the approval card file name.
        /// </summary>
        private const string ApprovalCardFileName = "ApprovalCard.json";

        /// <summary>
        /// Represents the updated approval card file name.
        /// </summary>
        private const string ApprovalUpdatedCardFileName = "ApprovalUpdatedCard.json";

        /// <summary>
        /// Represents the configure pair-up matches card file name.
        /// </summary>
        private const string ConfigurePairUpMatchesFileName = "ConfigurePairupMatchesCard.json";

        /// <summary>
        /// Represents the resume pair-up matches card file name.
        /// </summary>
        private const string ResumePairUpMatchesFileName = "ResumePairupMatchesCard.json";

        /// <summary>
        /// Represents the user pair-up teams mapping card file name.
        /// </summary>
        private const string PairUpTeamsMappingFileName = "PairupTeamsMappingCard.json";

        /// <summary>
        /// Instance to send logs to the Application Insights service.
        /// </summary>
        private readonly ILogger<CardHelper> logger;

        /// <summary>
        /// Cache for storing adaptive card json schema.
        /// </summary>
        private readonly IMemoryCache memoryCache;

        /// <summary>
        /// Information about the web hosting environment an application is running in.
        /// </summary>
        private readonly IWebHostEnvironment hostingEnvironment;

        /// <summary>
        /// The current culture's string localizer.
        /// </summary>
        private readonly IStringLocalizer<Strings> localizer;

        /// <summary>
        /// A set of key/value application bot configuration properties.
        /// </summary>
        private readonly IOptions<BotOptions> botOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="CardHelper"/> class.
        /// </summary>
        /// <param name="logger">Instance to send logs to the Application Insights service.</param>
        /// <param name="memoryCache">Cache for storing adaptive card json schema.</param>
        /// <param name="hostingEnvironment">Information about the web hosting environment an application is running in.</param>
        /// <param name="localizer">The current culture's string localizer.</param>
        /// <param name="botOptions">A set of key/value application bot configuration properties.</param>
        public CardHelper(
            ILogger<CardHelper> logger,
            IMemoryCache memoryCache,
            IWebHostEnvironment hostingEnvironment,
            IStringLocalizer<Strings> localizer,
            IOptions<BotOptions> botOptions)
        {
            this.logger = logger;
            this.memoryCache = memoryCache;
            this.hostingEnvironment = hostingEnvironment;
            this.localizer = localizer;
            this.botOptions = botOptions;
        }

        /// <summary>
        /// This method will construct the welcome notification card for personal scope.
        /// </summary>
        /// <returns>Welcome notification card attachment.</returns>
        public Attachment GetWelcomeNotificationCard()
        {
            this.logger.LogInformation("Get welcome notification card initiated.");
            var welcomeCardDataContents = new WelcomeCardData()
            {
                WelcomeTitleText = this.localizer.GetString("WelcomeTitleText"),
                WelcomeHeaderText = this.localizer.GetString("WelcomeHeaderText"),
                DiscoverGroupsBulletText = this.localizer.GetString("DiscoverGroupsBulletText"),
                MeetPeopleBulletText = this.localizer.GetString("MeetPeopleBulletText"),
                GetAnswersBulletText = this.localizer.GetString("GetAnswersBulletText"),
                AboutGroupsBulletText = this.localizer.GetString("AboutGroupsBulletText"),
                DiscoverGroupsButtonText = this.localizer.GetString("DiscoverGroupsButtonText"),
                DiscoverGroupsUrl = new Uri($"{TabRedirectionUrl}/{this.botOptions.Value.ManifestId}/{TabEntityId}").ToString(),
            };

            var cardTemplate = this.GetCardTemplate(CardCacheConstants.WelcomeJsonTemplate, WelcomeCardFileName);

            var template = new AdaptiveCardTemplate(cardTemplate);
            var card = template.Expand(welcomeCardDataContents);

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = AdaptiveCard.FromJson(card).Card,
            };

            this.logger.LogInformation("Get welcome notification card succeeded.");

            return attachment;
        }

        /// <summary>
        /// This method will construct the QnA response notification card for user.
        /// </summary>
        /// <param name="question">Question from the user.</param>
        /// <param name="answer">Knowledge base answer from QnA maker service.</param>
        /// <param name="prompts">Prompts associated with the current question.</param>
        /// <param name="appBaseUri">The base URI where the App is hosted.</param>
        /// <returns>QnA response notification card attachment.</returns>
        public Attachment GetQnAResponseNotificationCard(string question, string answer, IList<PromptDTO> prompts, string appBaseUri)
        {
            this.logger.LogInformation("Get QnA response notification card initiated.");
            var qnAResponseCardContents = new QnAResponseCardData()
            {
                ResponseHeaderText = this.localizer.GetString("ResponseHeaderText"),
                QuestionText = question,
                AnswerText = answer,
                IsPromptQuestionsPresent = prompts.Count > 0,
                PromptHeaderText = this.localizer.GetString("PromptHeaderText"),
                ShareFeedbackButtonText = this.localizer.GetString("ShareFeedbackText"),
                FeedbackHeaderText = this.localizer.GetString("FeedbackHeaderText"),
                FeedbackTitleText = this.localizer.GetString("FeedbackTitleText"),
                HelpfulTitleText = this.localizer.GetString("HelpfulTitleText"),
                NeedsImprovementTitleText = this.localizer.GetString("NeedsImprovementTitleText"),
                NotHelpfulTitleText = this.localizer.GetString("NotHelpfulTitleText"),
                ChoiceSetPlaceholder = this.localizer.GetString("ChoiceSetPlaceholder"),
                DescriptionText = this.localizer.GetString("FeedbackDescriptionTitleText"),
                DescriptionPlaceHolderText = this.localizer.GetString("DescriptionPlaceHolderText"),
                ShareButtonText = this.localizer.GetString("ShareButtonText"),
                ShareCommand = Constants.ShareCommand,
            };

            if (prompts.Count != 0)
            {
                var promptLimit = prompts.Count > MaximumPromptQuestionCount ? prompts.Take(MaximumPromptQuestionCount) : prompts;
                qnAResponseCardContents.ColumnSets = new List<ColumnData>();

                foreach (var prompt in promptLimit)
                {
                    qnAResponseCardContents.ColumnSets.Add(new ColumnData()
                    {
                        ImageUrl = new Uri($"{appBaseUri}/Artifacts/Info.png"),
                        PromptQuestion = prompt.DisplayText,
                    });
                }
            }

            var cardTemplate = this.GetCardTemplate(CardCacheConstants.QnAResponseJsonTemplate, QnAResponseCardFileName);

            var template = new AdaptiveCardTemplate(cardTemplate);
            var card = template.Expand(qnAResponseCardContents);

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = AdaptiveCard.FromJson(card).Card,
            };

            this.logger.LogInformation("Get QnA response notification card succeeded.");

            return attachment;
        }

        /// <summary>
        /// This method will construct the share feedback notification card for admin team.
        /// </summary>
        /// <param name="feedbackData">User activity payload.</param>
        /// <param name="userDetails">User details.</param>
        /// <returns>Share feedback notification card attachment.</returns>
        public Attachment GetShareFeedbackNotificationCard(SubmitActionDataForTeamsBehavior feedbackData, TeamsChannelAccount userDetails)
        {
            this.logger.LogInformation("Get share feedback notification card initiated.");
            var shareFeedbackCardContents = new ShareFeedbackCardData()
            {
                FeedbackText = this.localizer.GetString("Feedback"),
                FeedbackSubHeaderText = this.localizer.GetString("FeedbackSubHeaderText", userDetails.GivenName),
                FeedbackType = feedbackData.FeedbackType,
                DescriptionText = this.localizer.GetString("FeedbackDescriptionTitleText"),
                FeedbackDescription = feedbackData.FeedbackDescription,
                CreatedOnText = this.localizer.GetString("CreatedOn"),
                FeedbackCreatedDate = DateTime.UtcNow.ToShortDateString(),
                ChatWithUserButtonText = this.localizer.GetString("ChatWithMatchButtonText", userDetails.GivenName),
                ChatInitiateURL = new Uri($"{Constants.ChatInitiateURL}?users={Uri.EscapeDataString(userDetails.UserPrincipalName)}&message={Uri.EscapeDataString(Strings.InitiateChatText)}").ToString(),
            };

            var cardTemplate = this.GetCardTemplate(CardCacheConstants.ShareFeedbackJsonTemplate, ShareFeedbackCardFileName);

            var template = new AdaptiveCardTemplate(cardTemplate);
            var card = template.Expand(shareFeedbackCardContents);

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = AdaptiveCard.FromJson(card).Card,
            };

            this.logger.LogInformation("Get share feedback notification card succeeded.");

            return attachment;
        }

        /// <summary>
        /// Get approval notification card attachment.
        /// </summary>
        /// <param name="groupEntity">Employee resource group entity.</param>
        /// <param name="groupCreatorName">Group creator name.</param>
        /// <param name="approvalStatusText">Represents the approval status text.</param>
        /// <returns>Approval notification card attachment.</returns>
        public Attachment GetApprovalCard(EmployeeResourceGroupEntity groupEntity, string groupCreatorName, string approvalStatusText = null)
        {
            string cardTemplate;
            this.logger.LogInformation("Get approval card initiated.");
            var approvalCardContents = new ApprovalCardData()
            {
                RequestSubmittedText = this.localizer.GetString("ApproveCardRequestSubmitted"),
                ApprovalStatusText = string.IsNullOrEmpty(approvalStatusText) ? this.localizer.GetString("PendingApprovalText") : approvalStatusText,
                ApprovalStatus = groupEntity.ApprovalStatus.ToString(),
                GroupDescriptionText = groupEntity.GroupDescription,
                NameText = this.localizer.GetString("NameText"),
                GroupNameText = groupEntity.GroupName,
                TagsText = this.localizer.GetString("TagText"),
                ApproveTagsName = !string.IsNullOrEmpty(groupEntity.Tags) ? string.Join(", ", JsonConvert.DeserializeObject<List<string>>(groupEntity.Tags)) : string.Empty,
                LocationText = this.localizer.GetString("LocationText"),
                LocationName = groupEntity.Location,
                CreatedByNameText = this.localizer.GetString("CreatedByNameText"),
                CreatedByName = groupCreatorName,
                SearchEnableText = this.localizer.GetString("SearchEnabledText"),
                SearchEnableStatusText = groupEntity.IncludeInSearchResults ? this.localizer.GetString("YesText") : this.localizer.GetString("NoText"),
                ApproveButtonText = this.localizer.GetString("ApproveButtonText"),
                RejectButtonText = this.localizer.GetString("RejectButtonText"),
                ApprovedCommandText = Constants.ApprovedText,
                RejectCommandText = Constants.RejectedText,
                GroupId = groupEntity.GroupId,
            };

            if (groupEntity.ApprovalStatus == (int)ApprovalStatus.PendingForApproval)
            {
                cardTemplate = this.GetCardTemplate(CardCacheConstants.AdminNotificationCardJsonTemplate, ApprovalCardFileName);
            }
            else
            {
                cardTemplate = this.GetCardTemplate(CardCacheConstants.AdminApprovalCardJsonTemplate, ApprovalUpdatedCardFileName);
            }

            var template = new AdaptiveCardTemplate(cardTemplate);
            var card = template.Expand(approvalCardContents);

            AdaptiveCard adaptiveCard = AdaptiveCard.FromJson(card).Card;
            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = adaptiveCard,
            };

            this.logger.LogInformation("Get approval card succeeded.");

            return attachment;
        }

        /// <summary>
        /// This method will construct the configure user pair-up notification card for user.
        /// </summary>
        /// <returns>Configure user pair-up notification card attachment.</returns>
        public Attachment GetConfigureMatchesNotificationCard()
        {
            this.logger.LogInformation("Get configure matches notification card initiated.");
            var configureMatchesCardContents = new ConfigurePairupMatchesCardData()
            {
                ConfigureMatchesCardTitle = this.localizer.GetString("ConfigureNotificationCardTitle"),
                ConfigureMatchesButtonText = this.localizer.GetString("ConfigureNotificationCardButtonText"),
                ConfigureMatchesCommand = Constants.ConfigureMatchesCommand,
            };

            var cardTemplate = this.GetCardTemplate(CardCacheConstants.ConfigureMatchesJsonTemplate, ConfigurePairUpMatchesFileName);

            var template = new AdaptiveCardTemplate(cardTemplate);
            var card = template.Expand(configureMatchesCardContents);

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = AdaptiveCard.FromJson(card).Card,
            };

            this.logger.LogInformation("Get configure matches notification card succeeded.");

            return attachment;
        }

        /// <summary>
        /// This method will construct resume pair-up notification card for user.
        /// </summary>
        /// <returns>Resume pair-up notification attachment.</returns>
        public Attachment GetResumePairupNotificationCard()
        {
            this.logger.LogInformation("Get resume pair-up notification card initiated.");
            var updateMatchesCardContents = new ResumePairupMatchesCardData()
            {
                UpdateCardTitle = this.localizer.GetString("UpdateCardTitle"),
                UpdateCardButtonText = this.localizer.GetString("UpdateCardButtonTitle"),
                ConfigureMatchesCommand = Constants.ConfigureMatchesCommand,
            };

            var cardTemplate = this.GetCardTemplate(CardCacheConstants.ResumeMatchesJsonTemplate, ResumePairUpMatchesFileName);

            var template = new AdaptiveCardTemplate(cardTemplate);
            var card = template.Expand(updateMatchesCardContents);

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = AdaptiveCard.FromJson(card).Card,
            };

            this.logger.LogInformation("Get resume pair-up notification card succeeded.");

            return attachment;
        }

        /// <summary>
        /// This method will construct user pair-up matches configuration card, user will have option to pause/resume pair-up matches for Teams.
        /// </summary>
        /// <param name="teamMappingsForRecipient">Team mappings for recipient of configure pair-up matches card.</param>
        /// <param name="userTeamMappingEntities">Teams pair up mapping entities.</param>
        /// <returns>User pair-up card attachment.</returns>
        public Attachment GetUserPairUpMatchesCard(IEnumerable<TeamPairUpData> teamMappingsForRecipient, IEnumerable<TeamUserPairUpMappingEntity> userTeamMappingEntities)
        {
            teamMappingsForRecipient = teamMappingsForRecipient ?? throw new ArgumentNullException(nameof(teamMappingsForRecipient));
            userTeamMappingEntities = userTeamMappingEntities ?? throw new ArgumentNullException(nameof(userTeamMappingEntities));

            if (!teamMappingsForRecipient.Any() || !userTeamMappingEntities.Any())
            {
                this.logger.LogInformation("Team mappings for recipient card is empty or null.");

                return null;
            }

            this.logger.LogInformation("Get user pair-up matches card initiated.");

            // Comma separated team id's user opted out from pair up matches to display in configure pair up Adaptive card.
            var teamsOptedOutPairUpMatches = string.Join(",", userTeamMappingEntities.Where(row => row.IsPaused).Select(id => id.TeamId));
            var userMatchesCardContents = new UserPairUpMatchesNotificationCardData()
            {
                ConfigureUserMatchesCardTitleText = this.localizer.GetString("ConfigureUserMatchesCardTitle"),
                ConfigureUserMatchesButtonText = this.localizer.GetString("ConfigureUserMatchesCardButtonText"),
                TeamPairUpEntities = teamMappingsForRecipient,
                CommaSeparatedTeamIds = teamsOptedOutPairUpMatches,
                UpdateMatchesCommand = Constants.UpdateMatchesCommand,
            };

            var cardTemplate = this.GetCardTemplate(CardCacheConstants.PairUpTeamsMappingJsonTemplate, PairUpTeamsMappingFileName);

            var template = new AdaptiveCardTemplate(cardTemplate);
            var card = template.Expand(userMatchesCardContents);

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = AdaptiveCard.FromJson(card).Card,
            };

            this.logger.LogInformation("Get user pair-up matches card succeeded.");

            return attachment;
        }

        /// <summary>
        /// Get card template from memory.
        /// </summary>
        /// <param name="cardCacheKey">Card cache key.</param>
        /// <param name="cardJsonTemplateFileName">File name of JSON adaptive card template with file extension as .json to be provided.</param>
        /// <returns>Returns JSON adaptive card template string.</returns>
        private string GetCardTemplate(string cardCacheKey, string cardJsonTemplateFileName)
        {
            this.logger.LogInformation("Get card template initiated.");

            bool isCacheEntryExists = this.memoryCache.TryGetValue(cardCacheKey, out string cardTemplate);

            if (!isCacheEntryExists)
            {
                var cardJsonFilePath = Path.Combine(this.hostingEnvironment.ContentRootPath, $".\\Cards\\{cardJsonTemplateFileName}");
                cardTemplate = File.ReadAllText(cardJsonFilePath);
                this.memoryCache.Set(cardCacheKey, cardTemplate);
            }

            this.logger.LogInformation("Get card template succeeded.");

            return cardTemplate;
        }
    }
}
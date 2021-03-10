// <copyright file="DIConnectBot.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Bot
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Teams;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.DIConnect.Common;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.EmployeeResourceGroup;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.FeedbackData;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.UserPairupMapping;
    using Microsoft.Teams.Apps.DIConnect.Common.Resources;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.CommonBot;
    using Microsoft.Teams.Apps.DIConnect.DIConnect.Common.FeedbackData;
    using Microsoft.Teams.Apps.DIConnect.Helpers;
    using Microsoft.Teams.Apps.DIConnect.Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// DI Connect Bot.
    /// Captures user data, team data, upload files.
    /// </summary>
    public class DIConnectBot : TeamsActivityHandler
    {
        /// <summary>
        /// Sets the height of the error message task module.
        /// </summary>
        private const int ErrorMessageTaskModuleHeight = 800;

        /// <summary>
        /// Sets the width of the error message task module.
        /// </summary>
        private const int ErrorMessageTaskModuleWidth = 600;

        /// <summary>
        /// Represent for group created command.
        /// </summary>
        private const string GroupCreatedCommand = "Group created";

        /// <summary>
        /// Represents the conversation type as channel.
        /// </summary>
        private const string ChannelConversationType = "channel";

        /// <summary>
        /// Represents the conversation type as personal.
        /// </summary>
        private const string PersonalConversationType = "personal";

        /// <summary>
        /// Represents the Team renamed event type.
        /// </summary>
        private static readonly string TeamRenamedEventType = "teamRenamed";

        /// <summary>
        /// Represents the Team data capture.
        /// </summary>
        private readonly TeamsDataCapture teamsDataCapture;

        /// <summary>
        /// Represents the Team file upload.
        /// </summary>
        private readonly TeamsFileUpload teamsFileUpload;

        /// <summary>
        /// Represents the feedback data repository.
        /// </summary>
        private readonly FeedbackDataRepository feedbackDataRepository;

        /// <summary>
        /// Response generating from QnA maker knowledge base service.
        /// </summary>
        private readonly KnowledgeBaseResponse knowledgeBaseResponse;

        /// <summary>
        /// Helper for working with bot notification card.
        /// </summary>
        private readonly NotificationCardHelper notificationCardHelper;

        /// <summary>
        /// A set of key/value application bot configuration properties.
        /// </summary>
        private readonly IOptions<BotOptions> botOptions;

        /// <summary>
        /// The current culture's string localizer.
        /// </summary>
        private readonly IStringLocalizer<Strings> localizer;

        /// <summary>
        /// The current culture's string localizer.
        /// </summary>
        private readonly AdminTeamNotifier teamNotification;

        /// <summary>
        /// Instance of employee resource group repository.
        /// </summary>
        private readonly EmployeeResourceGroupRepository employeeResourceGroupRepository;

        /// <summary>
        /// Repository for team pair matching storage operations.
        /// </summary>
        private readonly TeamUserPairUpMappingRepository teamUserPairUpMappingRepository;

        /// <summary>
        /// Instance of class that handles adaptive card helper methods.
        /// </summary>
        private readonly CardHelper cardHelper;

        /// <summary>
        /// Helper class for getting user mappings for a team.
        /// </summary>
        private readonly UserTeamMappingsHelper userTeamMappingsHelper;

        /// <summary>
        /// Instance to send logs to the Application Insights service.
        /// </summary>
        private readonly ILogger<DIConnectBot> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DIConnectBot"/> class.
        /// </summary>
        /// <param name="logger">Instance to send logs to the Application Insights service.</param>
        /// <param name="teamsDataCapture">Teams data capture service.</param>
        /// <param name="employeeResourceGroupRepository">Instance of employee resource group repository.</param>
        /// <param name="teamsFileUpload">Teams file upload service.</param>
        /// <param name="teamNotification">Send team notification service.</param>
        /// <param name="knowledgeBaseResponse">Knowledge base response instance.</param>
        /// <param name="feedbackDataRepository">Feedback data repository instance.</param>
        /// <param name="notificationCardHelper">Notification card helper instance.</param>
        /// <param name="botOptions">A set of key/value application bot configuration properties.</param>
        /// <param name="localizer">Localization service.</param>
        /// <param name="cardHelper">Instance of class that handles adaptive card helper methods.</param>
        /// <param name="teamUserPairUpMappingRepository">Instance of team pair-up repository to access user pair-up matches.</param>
        /// <param name="userTeamMappingsHelper">Instance of helper for user mappings for a Team.</param>
        public DIConnectBot(
            ILogger<DIConnectBot> logger,
            TeamsDataCapture teamsDataCapture,
            EmployeeResourceGroupRepository employeeResourceGroupRepository,
            TeamsFileUpload teamsFileUpload,
            KnowledgeBaseResponse knowledgeBaseResponse,
            FeedbackDataRepository feedbackDataRepository,
            NotificationCardHelper notificationCardHelper,
            IOptions<BotOptions> botOptions,
            IStringLocalizer<Strings> localizer,
            AdminTeamNotifier teamNotification,
            TeamUserPairUpMappingRepository teamUserPairUpMappingRepository,
            CardHelper cardHelper,
            UserTeamMappingsHelper userTeamMappingsHelper)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.teamsDataCapture = teamsDataCapture ?? throw new ArgumentNullException(nameof(teamsDataCapture));
            this.employeeResourceGroupRepository = employeeResourceGroupRepository ?? throw new ArgumentNullException(nameof(employeeResourceGroupRepository));
            this.teamsFileUpload = teamsFileUpload ?? throw new ArgumentNullException(nameof(teamsFileUpload));
            this.knowledgeBaseResponse = knowledgeBaseResponse ?? throw new ArgumentNullException(nameof(knowledgeBaseResponse));
            this.feedbackDataRepository = feedbackDataRepository ?? throw new ArgumentNullException(nameof(feedbackDataRepository));
            this.notificationCardHelper = notificationCardHelper ?? throw new ArgumentNullException(nameof(notificationCardHelper));
            this.botOptions = botOptions ?? throw new ArgumentNullException(nameof(botOptions));
            this.teamNotification = teamNotification ?? throw new ArgumentNullException(nameof(teamNotification));
            this.localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            this.botOptions = botOptions ?? throw new ArgumentNullException(nameof(botOptions));
            this.teamUserPairUpMappingRepository = teamUserPairUpMappingRepository ?? throw new ArgumentNullException(nameof(teamUserPairUpMappingRepository));
            this.cardHelper = cardHelper ?? throw new ArgumentNullException(nameof(cardHelper));
            this.userTeamMappingsHelper = userTeamMappingsHelper ?? throw new ArgumentNullException(nameof(userTeamMappingsHelper));
        }

        /// <summary>
        /// Invoked when a conversation update activity is received from the channel.
        /// </summary>
        /// <param name="turnContext">The context object for this turn.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        protected override async Task OnConversationUpdateActivityAsync(
            ITurnContext<IConversationUpdateActivity> turnContext,
            CancellationToken cancellationToken)
        {
            // base.OnConversationUpdateActivityAsync is useful when it comes to responding to users being added to or removed from the conversation.
            // For example, a bot could respond to a user being added by greeting the user.
            // By default, base.OnConversationUpdateActivityAsync will call <see cref="OnMembersAddedAsync(IList{ChannelAccount}, ITurnContext{IConversationUpdateActivity}, CancellationToken)"/>
            // if any users have been added or <see cref="OnMembersRemovedAsync(IList{ChannelAccount}, ITurnContext{IConversationUpdateActivity}, CancellationToken)"/>
            // if any users have been removed. base.OnConversationUpdateActivityAsync checks the member ID so that it only responds to updates regarding members other than the bot itself.
            await base.OnConversationUpdateActivityAsync(turnContext, cancellationToken);

            var activity = turnContext.Activity;

            var isTeamRenamed = this.IsTeamInformationUpdated(activity);
            if (isTeamRenamed)
            {
                await this.teamsDataCapture.OnTeamInformationUpdatedAsync(activity);
            }

            if (activity.MembersAdded != null)
            {
                await this.teamsDataCapture.OnBotAddedAsync(turnContext, activity);
            }

            if (activity.MembersRemoved != null)
            {
                await this.teamsDataCapture.OnBotRemovedAsync(activity);
            }
        }

        /// <summary>
        /// Handle when a message is addressed to the bot.
        /// </summary>
        /// <param name="turnContext">Context object containing information cached for a single turn of conversation with a user.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>A Task resolving to either a login card or the adaptive card of the Reddit post.</returns>
        /// <remarks>
        /// For more information on bot messaging in Teams, see the documentation
        /// https://docs.microsoft.com/en-us/microsoftteams/platform/bots/how-to/conversations/conversation-basics?tabs=dotnet#receive-a-message .
        /// </remarks>
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            turnContext = turnContext ?? throw new ArgumentNullException(nameof(turnContext));
            var activity = turnContext.Activity;
            try
            {
                string command;
                SubmitActionDataForTeamsBehavior valuesFromCard = null;
                if (string.IsNullOrWhiteSpace(activity.Text) && JObject.Parse(activity.Value?.ToString())["command"].ToString() == null)
                {
                    return;
                }

                // We are supporting pause/resume matches either from bot command or Adaptive submit card action.
                if (string.IsNullOrWhiteSpace(activity.Text) && activity.Value != null)
                {
                    valuesFromCard = ((JObject)activity.Value).ToObject<SubmitActionDataForTeamsBehavior>();
                    command = valuesFromCard.Command.Trim();
                }
                else
                {
                    command = activity.Text.Trim();
                }

                if (activity.Conversation.ConversationType == PersonalConversationType)
                {
                    // Command to get configure pair-up matches notification card.
                    if (command.Equals(this.localizer.GetString("PauseMatchesCommand"), StringComparison.CurrentCultureIgnoreCase)
                        || command == Constants.PauseMatchesCommand)
                    {
                        if (activity.Value == null)
                        {
                            // Send user matches card.
                            await this.userTeamMappingsHelper.SendUserTeamMappingsCardAsync(turnContext, cancellationToken);
                            return;
                        }

                        if (valuesFromCard.TeamId != null)
                        {
                            var userPairUpMappingEntity = await this.teamUserPairUpMappingRepository.GetAsync(activity.From.AadObjectId, valuesFromCard.TeamId);
                            userPairUpMappingEntity.IsPaused = true;
                            await this.teamUserPairUpMappingRepository.InsertOrMergeAsync(userPairUpMappingEntity);
                            var configureUserMatchesNoticationCard = MessageFactory.Attachment(this.cardHelper.GetConfigureMatchesNotificationCard());
                            await turnContext.SendActivityAsync(configureUserMatchesNoticationCard, cancellationToken);

                            return;
                        }
                    }

                    // Command to get user matches card.
                    else if (command.Equals(this.localizer.GetString("ConfigureMatchesCommand"), StringComparison.CurrentCultureIgnoreCase)
                        || command == Constants.ConfigureMatchesCommand)
                    {
                        await this.userTeamMappingsHelper.SendUserTeamMappingsCardAsync(turnContext, cancellationToken);

                        return;
                    }

                    // Command to update user pair-ups.
                    else if (command.Equals(this.localizer.GetString("UpdateMatchesCommand"), StringComparison.CurrentCultureIgnoreCase)
                        || command == Constants.UpdateMatchesCommand)
                    {
                        if (activity.Value == null)
                        {
                            // Send user matches card.
                            await this.userTeamMappingsHelper.SendUserTeamMappingsCardAsync(turnContext, cancellationToken);
                            return;
                        }

                        // Adaptive card submit action sends back only team id's which are being checked.
                        // Explicitly setting choice set as empty array of string when all the choices are unchecked,
                        // to update the IsPaused flag as 'false' for all the teams where user is a member.
                        var choiceSet = JObject.Parse(activity.Value.ToString())["choiceset"] != null
                            ? JObject.Parse(activity.Value?.ToString())["choiceset"].ToString().Split(",")
                            : new string[0];

                        var userTeamMappings = await this.teamUserPairUpMappingRepository.GetAllAsync(activity.From.AadObjectId);

                        foreach (var teamUserPair in userTeamMappings)
                        {
                            teamUserPair.IsPaused = choiceSet.Contains(teamUserPair.TeamId);
                            await this.teamUserPairUpMappingRepository.InsertOrMergeAsync(teamUserPair);
                        }

                        var updateConfigurePairupCard = MessageFactory.Attachment(this.cardHelper.GetResumePairupNotificationCard());
                        await turnContext.SendActivityAsync(updateConfigurePairupCard, cancellationToken);

                        return;
                    }
                    else if (command == Constants.ShareCommand)
                    {
                        if (valuesFromCard != null
                            && !string.IsNullOrWhiteSpace(valuesFromCard.FeedbackDescription)
                            && !string.IsNullOrWhiteSpace(valuesFromCard.FeedbackType))
                        {
                            TeamsChannelAccount userDetails = await this.GetConversationUserDetailAsync(turnContext, cancellationToken);
                            var teamNotificationAttachment = this.cardHelper.GetShareFeedbackNotificationCard(valuesFromCard, userDetails);
                            var feedbackEntity = new FeedbackEntity
                            {
                                Feedback = valuesFromCard.FeedbackDescription,
                                FeedbackId = Guid.NewGuid().ToString(),
                                UserAadObjectId = userDetails.AadObjectId,
                                SubmittedOn = DateTime.UtcNow,
                            };

                            this.logger.LogInformation("Feedback submitted successfully");
                            await this.notificationCardHelper.SendProactiveNotificationCardAsync(teamNotificationAttachment, this.botOptions.Value.AdminTeamId, activity.ServiceUrl);
                            await turnContext.SendActivityAsync(this.localizer.GetString("FeedbackSubmittedMessage"));
                            await this.feedbackDataRepository.InsertOrMergeAsync(feedbackEntity);
                        }
                    }
                    else
                    {
                        await this.knowledgeBaseResponse.SendReplyToQuestionAsync(turnContext, activity.Text);
                    }
                }
                else if (activity.Conversation.ConversationType == ChannelConversationType)
                {
                    if (activity.Value != null)
                    {
                        await this.teamNotification.UpdateGroupApprovalNotificationAsync(turnContext);
                    }
                    else
                    {
                        // Send help card for unsupported bot command.
                        await turnContext.SendActivityAsync(this.localizer.GetString("UnSupportedBotCommand"));
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Error while processing message request. {ex.Message}");
                await turnContext.SendActivityAsync(this.localizer.GetString("ErrorMessage"));
                return;
            }
        }

        /// <summary>
        /// Invoke when a file upload accept consent activity is received from the channel.
        /// </summary>
        /// <param name="turnContext">The context object for this turn.</param>
        /// <param name="fileConsentCardResponse">The accepted response object of File Card.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A task representing asynchronous operation.</returns>
        protected override async Task OnTeamsFileConsentAcceptAsync(
            ITurnContext<IInvokeActivity> turnContext,
            FileConsentCardResponse fileConsentCardResponse,
            CancellationToken cancellationToken)
        {
            var (fileName, notificationId) = this.teamsFileUpload.ExtractInformation(fileConsentCardResponse.Context);
            try
            {
                await this.teamsFileUpload.UploadToOneDrive(
                    fileName,
                    fileConsentCardResponse.UploadInfo.UploadUrl,
                    cancellationToken);

                await this.teamsFileUpload.FileUploadCompletedAsync(
                    turnContext,
                    fileConsentCardResponse,
                    fileName,
                    notificationId,
                    cancellationToken);
            }
            catch
            {
                await this.teamsFileUpload.FileUploadFailedAsync(
                    turnContext,
                    notificationId,
                    cancellationToken);
            }
        }

        /// <summary>
        /// Invoke when a file upload decline consent activity is received from the channel.
        /// </summary>
        /// <param name="turnContext">The context object for this turn.</param>
        /// <param name="fileConsentCardResponse">The declined response object of File Card.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A task representing asynchronous operation.</returns>
        protected override async Task OnTeamsFileConsentDeclineAsync(ITurnContext<IInvokeActivity> turnContext, FileConsentCardResponse fileConsentCardResponse, CancellationToken cancellationToken)
        {
            var (fileName, notificationId) = this.teamsFileUpload.ExtractInformation(
                fileConsentCardResponse.Context);

            await this.teamsFileUpload.CleanUp(
                turnContext,
                fileName,
                notificationId,
                cancellationToken);

            var reply = MessageFactory.Text(this.localizer.GetString("PermissionDeclinedText"));
            reply.TextFormat = "xml";
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }

        /// <summary>
        /// When OnTurn method receives a submit invoke activity on bot turn, it calls this method.
        /// </summary>
        /// <param name="turnContext">Provides context for a turn of a bot.</param>
        /// <param name="taskModuleRequest">Task module invoke request value payload.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents a task module response.</returns>
        protected override async Task<TaskModuleResponse> OnTeamsTaskModuleSubmitAsync(ITurnContext<IInvokeActivity> turnContext, TaskModuleRequest taskModuleRequest, CancellationToken cancellationToken)
        {
            if (turnContext == null || taskModuleRequest == null)
            {
                this.GetErrorPageTaskModule();
            }

            var taskModuleData = JsonConvert.DeserializeObject<SubmitActionDataForTeamsBehavior>(taskModuleRequest.Data?.ToString());
            if (taskModuleData == null)
            {
                this.GetErrorPageTaskModule();
            }

            // We are explicitly passing group id as null if 'IncludeInSearchResults' flag status as false from task module submit action.
            else if (
                taskModuleData.Command.Equals(GroupCreatedCommand, StringComparison.OrdinalIgnoreCase)
                && taskModuleData.GroupId != null)
            {
                var groupEntity = await this.employeeResourceGroupRepository.GetAsync(Constants.ResourceGroupTablePartitionKey, taskModuleData.GroupId);

                // Validating entity is exists and status is not approved/rejected to make sure the request is not processed before.
                if (groupEntity == null || groupEntity.ApprovalStatus != (int)ApprovalStatus.PendingForApproval)
                {
                    this.GetErrorPageTaskModule();
                }
                else
                {
                    groupEntity.IncludeInSearchResults = true;
                    await this.teamNotification.SendGroupApprovalNotificationAsync(groupEntity, turnContext.Activity.ServiceUrl, turnContext.Activity.From.Name);
                }
            }

            return null;
        }

        /// <summary>
        /// Get Teams channel account detailing user Azure Active Directory details.
        /// </summary>
        /// <param name="turnContext">Context object containing information cached for a single turn of conversation with a user.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        private async Task<TeamsChannelAccount> GetConversationUserDetailAsync(
          ITurnContext turnContext,
          CancellationToken cancellationToken)
        {
            turnContext = turnContext ?? throw new ArgumentNullException(nameof(turnContext));

            var members = await ((BotFrameworkAdapter)turnContext.Adapter).GetConversationMembersAsync(turnContext, cancellationToken);
            return JsonConvert.DeserializeObject<TeamsChannelAccount>(JsonConvert.SerializeObject(members.FirstOrDefault(member => member.Id == turnContext.Activity.From.Id)));
        }

        /// <summary>
        /// Return true or false if Teams information updated.
        /// </summary>
        /// <param name="activity">Conversation update activity.</param>
        /// <returns>Function return true if Teams information updated successfully otherwise returns false.</returns>
        private bool IsTeamInformationUpdated(IConversationUpdateActivity activity)
        {
            if (activity == null)
            {
                return false;
            }

            var channelData = activity.GetChannelData<TeamsChannelData>();
            if (channelData == null)
            {
                return false;
            }

            return DIConnectBot.TeamRenamedEventType.Equals(channelData.EventType, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Get error page task module.
        /// </summary>
        /// <returns>Return task module for error.</returns>
        private TaskModuleResponse GetErrorPageTaskModule()
        {
            return new TaskModuleResponse
            {
                Task = new TaskModuleContinueResponse
                {
                    Type = "continue",
                    Value = new TaskModuleTaskInfo()
                    {
                        Url = $"{this.botOptions.Value.AppBaseUri}/errorpage",
                        Height = ErrorMessageTaskModuleHeight,
                        Width = ErrorMessageTaskModuleWidth,
                        Title = this.localizer.GetString("AppTitle"),
                    },
                },
            };
        }
    }
}
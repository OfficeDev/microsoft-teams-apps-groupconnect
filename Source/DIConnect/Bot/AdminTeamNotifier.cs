// <copyright file="AdminTeamNotifier.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Bot
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.DIConnect.Common;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.EmployeeResourceGroup;
    using Microsoft.Teams.Apps.DIConnect.Common.Resources;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.CommonBot;
    using Microsoft.Teams.Apps.DIConnect.Helpers;
    using Microsoft.Teams.Apps.DIConnect.Models;
    using Newtonsoft.Json.Linq;
    using Polly;
    using Polly.Contrib.WaitAndRetry;
    using Polly.Retry;

    /// <summary>
    /// Send approval notification to admin team.
    /// </summary>
    public class AdminTeamNotifier
    {
        /// <summary>
        /// Represents retry delay.
        /// </summary>
        private const int RetryDelay = 1000;

        /// <summary>
        /// Represents retry count.
        /// </summary>
        private const int RetryCount = 2;

        /// <summary>
        /// Default value for channel activity to send notifications.
        /// </summary>
        private const string TeamsBotFrameworkChannelId = "msteams";

        /// <summary>
        /// A set of key/value application configuration properties for bot settings.
        /// </summary>
        private readonly IOptions<BotOptions> botOptions;

        /// <summary>
        /// Instance to send logs to the logger service.
        /// </summary>
        private readonly ILogger<AdminTeamNotifier> logger;

        /// <summary>
        /// Bot adapter.
        /// </summary>
        private readonly BotFrameworkHttpAdapter adapter;

        /// <summary>
        /// The current culture's string localizer.
        /// </summary>
        private readonly IStringLocalizer<Strings> localizer;

        /// <summary>
        /// Instance of employee resource group repository.
        /// </summary>
        private readonly EmployeeResourceGroupRepository employeeResourceGroupRepository;

        /// <summary>
        /// Retry policy with jitter, retry twice with a jitter delay of up to 1 sec. Retry for HTTP 429(transient error)/502 bad gateway.
        /// </summary>
        /// <remarks>
        /// Reference: https://github.com/Polly-Contrib/Polly.Contrib.WaitAndRetry#new-jitter-recommendation.
        /// </remarks>
        private readonly AsyncRetryPolicy retryPolicy = Policy.Handle<ErrorResponseException>(
            ex => ex.Response.StatusCode == HttpStatusCode.TooManyRequests || ex.Response.StatusCode == HttpStatusCode.InternalServerError)
            .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromMilliseconds(RetryDelay), RetryCount));

        /// <summary>
        /// Instance of class that handles user pair-up matches card helper methods.
        /// </summary>
        private readonly CardHelper cardHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminTeamNotifier"/> class.
        /// </summary>
        /// <param name="employeeResourceGroupRepository">Employee resource group data repository instance.</param>
        /// <param name="botOptions"> A set of key/value application bot configuration properties.</param>
        /// <param name="adapter">Bot adapter.</param>
        /// <param name="logger">Logger implementation to send logs to the logger service.</param>
        /// <param name="localizer">Localization service.</param>
        /// <param name="cardHelper">Instance of class that handles Approval card helper methods.</param>
        public AdminTeamNotifier(
            EmployeeResourceGroupRepository employeeResourceGroupRepository,
            IOptions<BotOptions> botOptions,
            BotFrameworkHttpAdapter adapter,
            ILogger<AdminTeamNotifier> logger,
            IStringLocalizer<Strings> localizer,
            CardHelper cardHelper)
        {
            this.employeeResourceGroupRepository = employeeResourceGroupRepository ?? throw new ArgumentNullException(nameof(employeeResourceGroupRepository));
            this.botOptions = botOptions ?? throw new ArgumentNullException(nameof(botOptions));
            this.adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            this.cardHelper = cardHelper ?? throw new ArgumentNullException(nameof(cardHelper));
        }

        /// <summary>
        /// Sends approval notification card to team.
        /// </summary>
        /// <param name="groupEntity">Employee resource group entity.</param>
        /// <param name="serviceBasePath">Service URL.</param>
        /// <param name="groupCreatorName">Group creator name.</param>
        /// <returns>A task representing asynchronous operation.</returns>
        public async Task SendGroupApprovalNotificationAsync(
            EmployeeResourceGroupEntity groupEntity,
            string serviceBasePath,
            string groupCreatorName)
        {
            var teamNotificationAttachment = this.cardHelper.GetApprovalCard(groupEntity, groupCreatorName);
            await this.SendProactiveNotificationCardAsync(teamNotificationAttachment, serviceBasePath);
        }

        /// <summary>
        /// Update group details with appropriate approval status.
        /// </summary>
        /// <param name="turnContext">Provides context for a turn of a bot.</param>
        /// <returns>A task representing asynchronous operation.</returns>
        public async Task UpdateGroupApprovalNotificationAsync(ITurnContext<IMessageActivity> turnContext)
        {
            var activity = turnContext?.Activity;
            var valuesFromCard = ((JObject)activity.Value).ToObject<SubmitActionDataForTeamsBehavior>();
            var groupEntity = await this.employeeResourceGroupRepository.GetAsync(
                Constants.ResourceGroupTablePartitionKey,
                valuesFromCard.GroupId);

            if (groupEntity == null)
            {
                return;
            }

            groupEntity.UpdatedOn = DateTime.UtcNow;
            groupEntity.UpdatedByObjectId = activity.From.AadObjectId;
            groupEntity.ApprovalStatus = valuesFromCard.Command.Equals(Constants.ApprovedText, StringComparison.OrdinalIgnoreCase) ? (int)ApprovalStatus.Approved : (int)ApprovalStatus.Rejected;
            groupEntity.IncludeInSearchResults = valuesFromCard.Command.Equals(Constants.ApprovedText, StringComparison.OrdinalIgnoreCase);
            await this.employeeResourceGroupRepository.InsertOrMergeAsync(groupEntity);

            string statusText = valuesFromCard.Command.Equals(Constants.ApprovedText, StringComparison.OrdinalIgnoreCase) ? this.localizer.GetString("ApprovedText") : this.localizer.GetString("RejectedText");
            IMessageActivity updateCard = MessageFactory.Attachment(this.cardHelper.GetApprovalCard(groupEntity, valuesFromCard.CreatedByName, statusText));
            updateCard.Id = activity.ReplyToId;
            await turnContext.UpdateActivityAsync(updateCard);
        }

        /// <summary>
        /// Send the given attachment to the specified admin Team.
        /// </summary>
        /// <param name="cardToSend">The attachment card to send.</param>
        /// <param name="serviceBasePath">Service URL for a particular team.</param>
        /// <returns>A task that sends notification card in channel.</returns>
        private async Task SendProactiveNotificationCardAsync(
            Attachment cardToSend,
            string serviceBasePath)
        {
            MicrosoftAppCredentials.TrustServiceUrl(serviceBasePath);
            var conversationReference = new ConversationReference()
            {
                ChannelId = TeamsBotFrameworkChannelId,
                Bot = new ChannelAccount() { Id = $"28:{this.botOptions.Value.MicrosoftAppId}" },
                ServiceUrl = serviceBasePath,
                Conversation = new ConversationAccount() { Id = this.botOptions.Value.AdminTeamId },
            };

            this.logger.LogInformation($"Sending notification to the specified conversation id- {this.botOptions.Value.AdminTeamId}");

            // Retry it in addition to the original call.
            await this.retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    await ((BotFrameworkAdapter)this.adapter).ContinueConversationAsync(
                            this.botOptions.Value.MicrosoftAppId,
                            conversationReference,
                            async (conversationTurnContext, conversationCancellationToken) =>
                            {
                                await conversationTurnContext.SendActivityAsync(MessageFactory.Attachment(cardToSend));
                            },
                            default);
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, $"Error while performing retry logic to send notification to the specified conversation id: {this.botOptions.Value.AdminTeamId}.");
                    throw;
                }
            });
        }
    }
}
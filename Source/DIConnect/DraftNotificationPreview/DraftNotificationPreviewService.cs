// <copyright file="DraftNotificationPreviewService.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.DraftNotificationPreview
{
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using AdaptiveCards;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.DIConnect.Bot;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.NotificationData;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.TeamData;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.AdaptiveCard;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.CommonBot;

    /// <summary>
    /// Draft notification preview service.
    /// </summary>
    public class DraftNotificationPreviewService
    {
        /// <summary>
        /// Represent Teams ID.
        /// </summary>
        private static readonly string MsTeamsChannelId = "msteams";

        /// <summary>
        /// Represent Channel conversation type.
        /// </summary>
        private static readonly string ChannelConversationType = "channel";

        /// <summary>
        /// Represent error response.
        /// </summary>
        private static readonly string ThrottledErrorResponse = "Throttled";

        /// <summary>
        /// Represent Bot application Id.
        /// </summary>
        private readonly string botAppId;

        /// <summary>
        /// Represent adaptive card creator.
        /// </summary>
        private readonly AdaptiveCardCreator adaptiveCardCreator;

        /// <summary>
        /// Represent variable for DIConnectBotAdapter Interface.
        /// </summary>
        private readonly DIConnectBotAdapter diConnectBotAdapter;

        /// <summary>
        /// Initializes a new instance of the <see cref="DraftNotificationPreviewService"/> class.
        /// </summary>
        /// <param name="botOptions">The bot options.</param>
        /// <param name="adaptiveCardCreator">Adaptive card creator service.</param>
        /// <param name="diConnectBotAdapter">Bot framework HTTP adapter instance.</param>
        public DraftNotificationPreviewService(
            IOptions<BotOptions> botOptions,
            AdaptiveCardCreator adaptiveCardCreator,
            DIConnectBotAdapter diConnectBotAdapter)
        {
            this.botAppId = botOptions.Value.MicrosoftAppId;
            if (string.IsNullOrEmpty(this.botAppId))
            {
                throw new ApplicationException("MicrosoftAppId setting is missing in the configuration.");
            }

            this.adaptiveCardCreator = adaptiveCardCreator;
            this.diConnectBotAdapter = diConnectBotAdapter;
        }

        /// <summary>
        /// Send a preview of a draft notification.
        /// </summary>
        /// <param name="draftNotificationEntity">Draft notification entity.</param>
        /// <param name="teamDataEntity">The team data entity.</param>
        /// <param name="teamsChannelId">The Teams channel id.</param>
        /// <returns>It returns HttpStatusCode.OK, if this method triggers the bot service to send the adaptive card successfully.
        /// It returns HttpStatusCode.TooManyRequests, if the bot service throttled the request to send the adaptive card.</returns>
        public async Task<HttpStatusCode> SendPreview(NotificationDataEntity draftNotificationEntity, TeamDataEntity teamDataEntity, string teamsChannelId)
        {
            if (draftNotificationEntity == null)
            {
                throw new ArgumentException("Null draft notification entity.");
            }

            if (teamDataEntity == null)
            {
                throw new ArgumentException("Null team data entity.");
            }

            if (string.IsNullOrWhiteSpace(teamsChannelId))
            {
                throw new ArgumentException("Null channel id.");
            }

            // Create bot conversation reference.
            var conversationReference = this.PrepareConversationReferenceAsync(teamDataEntity, teamsChannelId);

            // Ensure the bot service URL is trusted.
            if (!MicrosoftAppCredentials.IsTrustedServiceUrl(conversationReference.ServiceUrl))
            {
                MicrosoftAppCredentials.TrustServiceUrl(conversationReference.ServiceUrl);
            }

            // Trigger bot to send the adaptive card.
            try
            {
                await this.diConnectBotAdapter.ContinueConversationAsync(
                    this.botAppId,
                    conversationReference,
                    async (turnContext, cancellationToken) => await this.SendAdaptiveCardAsync(turnContext, draftNotificationEntity),
                    CancellationToken.None);
                return HttpStatusCode.OK;
            }
            catch (ErrorResponseException e)
            {
                var errorResponse = (ErrorResponse)e.Body;
                if (errorResponse != null
                    && errorResponse.Error.Code.Equals(DraftNotificationPreviewService.ThrottledErrorResponse, StringComparison.OrdinalIgnoreCase))
                {
                    return HttpStatusCode.TooManyRequests;
                }

                throw;
            }
        }

        /// <summary>
        /// Prepare conversation.
        /// </summary>
        /// <param name="teamDataEntity">Teams's data.</param>
        /// <param name="channelId">Teams's channel ID.</param>
        /// <returns>conversation Reference of chat/conversation.</returns>
        private ConversationReference PrepareConversationReferenceAsync(TeamDataEntity teamDataEntity, string channelId)
        {
            var channelAccount = new ChannelAccount
            {
                Id = $"28:{this.botAppId}",
            };

            var conversationAccount = new ConversationAccount
            {
                ConversationType = DraftNotificationPreviewService.ChannelConversationType,
                Id = channelId,
                TenantId = teamDataEntity.TenantId,
            };

            var conversationReference = new ConversationReference
            {
                Bot = channelAccount,
                ChannelId = DraftNotificationPreviewService.MsTeamsChannelId,
                Conversation = conversationAccount,
                ServiceUrl = teamDataEntity.ServiceUrl,
            };

            return conversationReference;
        }

        /// <summary>
        /// Send an adaptive card when bot trigger.
        /// </summary>
        /// <param name="turnContext">The context object for this turn.</param>
        /// <param name="draftNotificationEntity">Data for notifications that are either (depending on partition key):drafts,sent.</param>
        /// <returns>Return adaptive card as notification.</returns>
        private async Task SendAdaptiveCardAsync(
            ITurnContext turnContext,
            NotificationDataEntity draftNotificationEntity)
        {
            var reply = this.CreateReply(draftNotificationEntity);
            await turnContext.SendActivityAsync(reply);
        }

        /// <summary>
        /// Creates a notification reply object.
        /// </summary>
        /// <param name="draftNotificationEntity">Data for notifications that are either (depending on partition key):drafts,sent.</param>
        /// <returns>Return adaptive card as notification.</returns>
        private IMessageActivity CreateReply(NotificationDataEntity draftNotificationEntity)
        {
            var adaptiveCard = this.adaptiveCardCreator.CreateAdaptiveCard(
                draftNotificationEntity.Title,
                draftNotificationEntity.ImageLink,
                draftNotificationEntity.Summary,
                draftNotificationEntity.Author,
                draftNotificationEntity.ButtonTitle,
                draftNotificationEntity.ButtonLink);

            var attachment = new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = adaptiveCard,
            };

            var reply = MessageFactory.Attachment(attachment);

            return reply;
        }
    }
}
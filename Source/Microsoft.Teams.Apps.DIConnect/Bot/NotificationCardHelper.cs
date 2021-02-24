// <copyright file="NotificationCardHelper.cs" company="Microsoft Corporation">
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
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.CommonBot;
    using Polly;
    using Polly.Contrib.WaitAndRetry;
    using Polly.Retry;

    /// <summary>
    /// Class that helps to send notification cards.
    /// </summary>
    public class NotificationCardHelper
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
        private readonly ILogger<NotificationCardHelper> logger;

        /// <summary>
        /// The BotFramework adapter.
        /// </summary>
        private readonly BotFrameworkHttpAdapter adapter;

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
        /// Initializes a new instance of the <see cref="NotificationCardHelper"/> class.
        /// </summary>
        /// <param name="botOptions">A set of key/value application configuration properties.</param>
        /// <param name="logger">Logger implementation to send logs to the logger service.</param>
        /// <param name="adapter">Bot adapter.</param>
        public NotificationCardHelper(
            IOptions<BotOptions> botOptions,
            ILogger<NotificationCardHelper> logger,
            BotFrameworkHttpAdapter adapter)
        {
            this.botOptions = botOptions ?? throw new ArgumentNullException(nameof(botOptions));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
        }

        /// <summary>
        /// Send the given attachment to the specified conversation id.
        /// </summary>
        /// <param name="cardToSend">The attachment card to send.</param>
        /// <param name="conversationId">Conversation id where the notification have to be sent.</param>
        /// <param name="serviceUrl">Service URL.</param>
        /// <returns>A task that sends notification card.</returns>
        public async Task SendProactiveNotificationCardAsync(
            Attachment cardToSend,
            string conversationId,
            string serviceUrl)
        {
            MicrosoftAppCredentials.TrustServiceUrl(serviceUrl);
            var conversationReference = new ConversationReference()
            {
                ChannelId = TeamsBotFrameworkChannelId,
                Bot = new ChannelAccount() { Id = $"28:{this.botOptions.Value.MicrosoftAppId}" },
                ServiceUrl = serviceUrl,
                Conversation = new ConversationAccount() { Id = conversationId },
            };

            this.logger.LogInformation($"Sending notification to the specified conversation id- {conversationId}.");

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
                    this.logger.LogError(ex, $"Error while performing retry logic to send notification to the specified conversation id: {conversationId}.");
                    throw;
                }
            });
        }
    }
}
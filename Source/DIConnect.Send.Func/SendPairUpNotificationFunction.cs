// <copyright file="SendPairUpNotificationFunction.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Send.Func
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using AdaptiveCards;
    using AdaptiveCards.Templating;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.UserData;
    using Microsoft.Teams.Apps.DIConnect.Common.Resources;
    using Microsoft.Teams.Apps.DIConnect.Common.Services;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.MessageQueues.UserPairupQueue;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.Teams;
    using Microsoft.Teams.Apps.DIConnect.Send.Func.Cards;
    using Newtonsoft.Json;

    /// <summary>
    /// Azure Function App triggered by messages from a Service Bus queue
    /// used for sending pair up notification from the bot.
    /// </summary>
    public class SendPairUpNotificationFunction
    {
        /// <summary>
        /// Deep link to initiate chat.
        /// </summary>
        private const string ChatInitiateURL = "https://teams.microsoft.com/l/chat/0/0";

        /// <summary>
        /// Link to open meeting in teams.
        /// </summary>
        private const string MeetingLink = "https://teams.microsoft.com/l/meeting/new?subject=";

        /// <summary>
        /// Cache key for pair up notification card template.
        /// </summary>
        private const string PairUpCardJsonTemplate = "_PCTemplate";

        /// <summary>
        /// Represents the pair up notification card file name.
        /// </summary>
        private const string PairUpNotificationCardFileName = "PairUpNotificationCard.json";

        /// <summary>
        /// Represents the pause matches command.
        /// </summary>
        private const string PauseMatchesCommand = "Pause matches";

        /// <summary>
        /// Maximum number of attempts for retry while sending notification.
        /// </summary>
        private readonly int maxNumberOfAttempts = 2;

        /// <summary>
        /// Localization service.
        /// </summary>
        private readonly IStringLocalizer<Strings> localizer;

        /// <summary>
        /// App setting service.
        /// </summary>
        private readonly IAppSettingsService appSettingsService;

        /// <summary>
        /// Cache for storing authorization result.
        /// </summary>
        private readonly IMemoryCache memoryCache;

        /// <summary>
        /// Message service.
        /// </summary>
        private readonly IMessageService messageService;

        /// <summary>
        /// User data repository.
        /// </summary>
        private readonly UserDataRepository userDataRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendPairUpNotificationFunction"/> class.
        /// </summary>
        /// <param name="messageService">Message service.</param>
        /// <param name="userDataRepository">User data repository.</param>
        /// <param name="appSettingsService">App settings service.</param>
        /// <param name="memoryCache">MemoryCache instance for caching authorization result.</param>
        /// <param name="localizer">Localization service.</param>
        public SendPairUpNotificationFunction(
            IMessageService messageService,
            UserDataRepository userDataRepository,
            IAppSettingsService appSettingsService,
            IMemoryCache memoryCache,
            IStringLocalizer<Strings> localizer)
        {
            this.messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
            this.userDataRepository = userDataRepository ?? throw new ArgumentNullException(nameof(userDataRepository));
            this.appSettingsService = appSettingsService ?? throw new ArgumentNullException(nameof(appSettingsService));
            this.memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            this.localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        /// <summary>
        /// Azure Function App triggered by messages from a Service Bus queue
        /// used for sending pair up notification from the bot.
        /// </summary>
        /// <param name="myQueueItem">The Service Bus queue item.</param>
        /// <param name="log">The logger.</param>
        /// <param name="context">Execution context.</param>
        /// <returns>A <see cref="Task"/> Representing the asynchronous operation.</returns>
        [FunctionName("SendPairUpNotificationFunction")]
        public async Task Run(
            [ServiceBusTrigger(
                UserPairUpQueue.QueueName,
                Connection = UserPairUpQueue.ServiceBusConnectionConfigurationKey)]
            string myQueueItem,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation($"C# Pair-up matches notification send function processed message: {myQueueItem}");

            var messageContent = JsonConvert.DeserializeObject<UserPairUpQueueMessageContent>(myQueueItem);
            var serviceUrl = await this.appSettingsService.GetServiceUrlAsync();

            try
            {
                var recipient1Data = await this.userDataRepository.GetAsync(UserDataTableNames.UserDataPartition, messageContent.PairUpUserData.Recipient1.UserObjectId);
                var recipient2Data = await this.userDataRepository.GetAsync(UserDataTableNames.UserDataPartition, messageContent.PairUpUserData.Recipient2.UserObjectId);

                // Checking both recipient are present in user entity to make sure user is already installed the bot.
                if (recipient1Data != null && recipient1Data.ConversationId != null && recipient2Data != null && recipient2Data.ConversationId != null)
                {
                    // Send message recipient 1.
                    var recipient1Message = this.GetPairUpNotificationCard(
                        messageContent.PairUpUserData.Recipient1,
                        messageContent.PairUpUserData.Recipient2,
                        messageContent.TeamId,
                        messageContent.TeamName,
                        context.FunctionAppDirectory,
                        log);
                    var recipient1Response = await this.messageService.SendMessageAsync(
                        message: MessageFactory.Attachment(recipient1Message),
                        serviceUrl: serviceUrl,
                        conversationId: recipient1Data.ConversationId,
                        maxAttempts: this.maxNumberOfAttempts,
                        logger: log);

                    this.LogResponse(messageContent.PairUpUserData.Recipient1, recipient1Response, log);
                    log.LogInformation("Pairup notification card sent successfully");

                    // Send message to recipient 2.
                    var recipient2Message = this.GetPairUpNotificationCard(
                        messageContent.PairUpUserData.Recipient2,
                        messageContent.PairUpUserData.Recipient1,
                        messageContent.TeamId,
                        messageContent.TeamName,
                        context.FunctionAppDirectory,
                        log);
                    var recipient2Response = await this.messageService.SendMessageAsync(
                        message: MessageFactory.Attachment(recipient2Message),
                        serviceUrl: serviceUrl,
                        conversationId: recipient2Data.ConversationId,
                        maxAttempts: this.maxNumberOfAttempts,
                        logger: log);

                    this.LogResponse(messageContent.PairUpUserData.Recipient2, recipient2Response, log);
                    log.LogInformation("Pairup notification card sent successfully");
                }

                // Log warning here.
                else
                {
                    var message = $"\nReceipent1 object id {messageContent.PairUpUserData.Recipient1.UserObjectId} conversation id : {recipient1Data.ConversationId}" +
                    $"\nReceipent2 object id {messageContent.PairUpUserData.Recipient2.UserObjectId} conversation id : {recipient2Data.ConversationId}" +
                    $"from the team : {messageContent.TeamId}.";

                    log.LogWarning("Unable to send notification since recipients haven't installed the bot." + message);
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Failed to send message. ErrorMessage: {ex.GetType()}: {ex.Message}");
            }
        }

        /// <summary>
        /// Log notification response.
        /// </summary>
        /// <param name="userData">User information.</param>
        /// <param name="sendMessageResponse">Send notification response.</param>
        /// <param name="log">Logger.</param>
        private void LogResponse(
            UserData userData,
            SendMessageResponse sendMessageResponse,
            ILogger log)
        {
            if (sendMessageResponse.ResultType == SendMessageResult.Succeeded)
            {
                log.LogInformation($"Successfully sent the message." +
                    $"\nUser object id: {userData.UserObjectId}");
            }
            else
            {
                log.LogError($"Failed to send message." +
                    $"\nUser object id: {userData.UserObjectId}" +
                    $"\nResult: {sendMessageResponse.ResultType}." +
                    $"\nErrorMessage: {sendMessageResponse.ErrorMessage}.");
            }
        }

        /// <summary>
        /// Creates the pair-up notification card.
        /// </summary>
        /// <param name="sender">The user who will be sending this card.</param>
        /// <param name="recipient">The user who will be receiving this card.</param>
        /// <param name="teamId">Team id.</param>
        /// <param name="teamName">Team Name.</param>
        /// <param name="functionAppDirectory">Function app directory.</param>
        /// <param name="log">The logger.</param>
        /// <returns>Pair-up notification card.</returns>
        private Attachment GetPairUpNotificationCard(
            UserData sender,
            UserData recipient,
            string teamId,
            string teamName,
            string functionAppDirectory,
            ILogger log)
        {
            sender = sender ?? throw new ArgumentNullException(nameof(sender));
            recipient = recipient ?? throw new ArgumentNullException(nameof(recipient));

            log.LogInformation("Get pair-up notification card initiated.");
            var meetingTitle = this.localizer.GetString("MeetupTitle");
            var meetingContent = this.localizer.GetString("MeetupContent", this.localizer.GetString("AppTitle"));
            var meetingLink = $"{MeetingLink}{Uri.EscapeDataString(meetingTitle)}&attendees={recipient.UserPrincipalName}&content={Uri.EscapeDataString(meetingContent)}";

            var pairUpCardContents = new PairUpNotificationCardData()
            {
                MatchUpCardTitleText = this.localizer.GetString("MatchUpCardTitleContent"),
                MatchUpCardSubHeaderText = this.localizer.GetString("MatchUpCardMatchedText"),
                MatchUpCardContent = this.localizer.GetString("MatchUpCardContentPart1", recipient.UserGivenName, teamName),
                ChatWithUserButtonText = this.localizer.GetString("ChatWithMatchButtonText", recipient.UserGivenName),
                ChatInitiateURL = new Uri($"{ChatInitiateURL}?users={Uri.EscapeDataString(recipient.UserPrincipalName)}&message={Uri.EscapeDataString(this.localizer.GetString("InitiateChatText"))}").ToString(),
                ProposeMeetupButtonText = this.localizer.GetString("ProposeMeetupButtonText"),
                MeetingLink = new Uri(meetingLink).ToString(),
                PauseMatchesButtonText = this.localizer.GetString("PauseMatchesButtonText"),
                PauseMatchesText = PauseMatchesCommand,
                TeamId = teamId,
            };

            // Get pair up notification card template.
            bool isCacheEntryExists = this.memoryCache.TryGetValue(PairUpCardJsonTemplate, out string pairUpNotificationCardTemplate);

            if (!isCacheEntryExists)
            {
                var cardJsonFilePath = Path.Combine(functionAppDirectory, $".\\Cards\\{PairUpNotificationCardFileName}");
                pairUpNotificationCardTemplate = File.ReadAllText(cardJsonFilePath);
                this.memoryCache.Set(PairUpCardJsonTemplate, pairUpNotificationCardTemplate);
            }

            var template = new AdaptiveCardTemplate(pairUpNotificationCardTemplate);
            var card = template.Expand(pairUpCardContents);

            AdaptiveCard adaptiveCard = AdaptiveCard.FromJson(card).Card;
            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = adaptiveCard,
            };

            log.LogInformation("Get pair-up notification card succeeded.");

            return attachment;
        }
    }
}
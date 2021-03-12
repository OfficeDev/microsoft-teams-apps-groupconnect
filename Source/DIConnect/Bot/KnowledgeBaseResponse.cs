// <copyright file="KnowledgeBaseResponse.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Bot
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.DIConnect.Common.Resources;
    using Microsoft.Teams.Apps.DIConnect.Common.Services;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.CommonBot;
    using Microsoft.Teams.Apps.DIConnect.Helpers;

    /// <summary>
    /// Class to generate response of knowledge base.
    /// </summary>
    public class KnowledgeBaseResponse
    {
        /// <summary>
        /// Represents question and answer maker service provider.
        /// </summary>
        private readonly IQnAService qnaService;

        /// <summary>
        /// A set of key/value application bot configuration properties.
        /// </summary>
        private readonly IOptions<BotOptions> botOptions;

        /// <summary>
        /// The current culture's string localizer.
        /// </summary>
        private readonly IStringLocalizer<Strings> localizer;

        /// <summary>
        /// Instance of class that adaptive card helper methods.
        /// </summary>
        private readonly CardHelper cardHelper;

        /// <summary>
        /// Instance to send logs to the logger service.
        /// </summary>
        private readonly ILogger<KnowledgeBaseResponse> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="KnowledgeBaseResponse"/> class.
        /// </summary>
        /// <param name="qnaService">Question and answer maker service provider.</param>
        /// <param name="botOptions"> A set of key/value application bot configuration properties.</param>
        /// <param name="cardHelper">Instance of class that handles adaptive card helper methods.</param>
        /// <param name="localizer">Localization service.</param>
        /// <param name="logger">Logger implementation to send logs to the logger service.</param>
        public KnowledgeBaseResponse(
            IQnAService qnaService,
            IOptions<BotOptions> botOptions,
            CardHelper cardHelper,
            IStringLocalizer<Strings> localizer,
            ILogger<KnowledgeBaseResponse> logger)
        {
            this.qnaService = qnaService ?? throw new ArgumentNullException(nameof(qnaService));
            this.botOptions = botOptions ?? throw new ArgumentNullException(nameof(botOptions));
            this.cardHelper = cardHelper ?? throw new ArgumentNullException(nameof(cardHelper));
            this.localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get the reply to a question asked by end user.
        /// </summary>
        /// <param name="turnContext">Context object containing information cached for a single turn of conversation with user.</param>
        /// <param name="question">Question from user.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        public async Task SendReplyToQuestionAsync(
            ITurnContext<IMessageActivity> turnContext,
            string question)
        {
            try
            {
                var queryResult = await this.qnaService.GenerateAnswerAsync(question: question);

                if (queryResult == null || queryResult.Answers == null)
                {
                    this.logger.LogInformation($"Unable to get the reply to a question asked by end user - {question}.");
                    await turnContext.SendActivityAsync(this.localizer.GetString("NoMatchesFoundText"));
                }
                else if (queryResult.Answers.First().Id != -1)
                {
                    var answerData = queryResult.Answers.First();
                    await turnContext.SendActivityAsync(MessageFactory.Attachment(this.cardHelper.GetQnAResponseNotificationCard(
                        question: question,
                        answer: answerData.Answer,
                        prompts: answerData.Context.Prompts,
                        appBaseUri: this.botOptions.Value.AppBaseUri)));
                }
                else
                {
                    await turnContext.SendActivityAsync(this.localizer.GetString("NoMatchesFoundText"));
                }

                this.logger.LogInformation($"Send the reply to a question asked by end user - {question}.");
            }
            catch (Exception ex)
            {
                // Throw the error at calling place, if there is any generic exception which is not caught.
                this.logger.LogError(ex, $"Error while question asked by end user: {question}.");
                throw;
            }
        }
    }
}
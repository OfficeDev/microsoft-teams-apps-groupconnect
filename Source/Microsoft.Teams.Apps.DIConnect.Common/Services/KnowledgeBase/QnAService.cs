// <copyright file="QnAService.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Services
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.CognitiveServices.Knowledge.QnAMaker;
    using Microsoft.Azure.CognitiveServices.Knowledge.QnAMaker.Models;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories;

    /// <summary>
    /// QnA maker service provider class.
    /// </summary>
    public class QnAService : IQnAService
    {
        /// <summary>
        /// Repository for app config data activity.
        /// </summary>
        private readonly AppConfigRepository appConfigRepository;

        /// <summary>
        /// Represents a variable for IQnAMakerRuntimeClient Interface.
        /// </summary>
        private readonly IQnAMakerRuntimeClient qnaMakerRuntimeClient;

        /// <summary>
        /// Represents a set of key/value application configuration properties.
        /// </summary>
        private readonly QnAMakerSettings options;

        /// <summary>
        /// Initializes a new instance of the <see cref="QnAService"/> class.
        /// </summary>
        /// <param name="appConfigRepository">Repository for app config data activity.</param>
        /// <param name="optionsAccessor">A set of key/value application configuration properties.</param>
        /// <param name="qnaMakerRuntimeClient">QnA service runtime client.</param>
        public QnAService(
            AppConfigRepository appConfigRepository,
            IOptionsMonitor<QnAMakerSettings> optionsAccessor,
            IQnAMakerRuntimeClient qnaMakerRuntimeClient)
        {
            this.appConfigRepository = appConfigRepository ?? throw new ArgumentNullException(nameof(appConfigRepository));
            this.options = optionsAccessor.CurrentValue ?? throw new ArgumentNullException(nameof(optionsAccessor.CurrentValue));
            this.qnaMakerRuntimeClient = qnaMakerRuntimeClient ?? throw new ArgumentNullException(nameof(qnaMakerRuntimeClient));
        }

        /// <summary>
        /// Get answer from knowledge base for a given question.
        /// </summary>
        /// <param name="question">Question text.</param>
        /// <returns>QnA search result as response.</returns>
        public async Task<QnASearchResultList> GenerateAnswerAsync(string question)
        {
            var knowledgeBaseEntity = await this.appConfigRepository.GetAsync(AppConfigTableName.SettingsPartition, AppConfigTableName.KnowledgeBaseIdRowKey);

            if (knowledgeBaseEntity == null)
            {
                return null;
            }

            QnASearchResultList qnaSearchResult = await this.qnaMakerRuntimeClient.Runtime.GenerateAnswerAsync(knowledgeBaseEntity.Value, new QueryDTO()
            {
                Question = question.Trim(),
                ScoreThreshold = Convert.ToDouble(this.options.ScoreThreshold),
            });

            return qnaSearchResult;
        }
    }
}
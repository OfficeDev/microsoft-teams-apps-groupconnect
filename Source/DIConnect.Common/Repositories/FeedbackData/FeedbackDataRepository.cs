// <copyright file="FeedbackDataRepository.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Repositories.FeedbackData
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.DIConnect.DIConnect.Common.FeedbackData;

    /// <summary>
    /// Repository to store user feedback in the table storage.
    /// </summary>
    public class FeedbackDataRepository : BaseRepository<FeedbackEntity>
    {
        /// <summary>
        /// Feedback table name.
        /// </summary>
        private const string FeedbackTableName = "Feedback";

        /// <summary>
        /// Default partition key for feedback data repository.
        /// </summary>
        private const string TableDefaultPartitionKey = "Feedback";

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedbackDataRepository"/> class.
        /// </summary>
        /// <param name="logger">The logging service.</param>
        /// <param name="repositoryOptions">Options used to create the repository.</param>
        public FeedbackDataRepository(
            ILogger<FeedbackDataRepository> logger,
            IOptions<RepositoryOptions> repositoryOptions)
            : base(
                  logger,
                  storageAccountConnectionString: repositoryOptions.Value.StorageAccountConnectionString,
                  tableName: FeedbackTableName,
                  defaultPartitionKey: TableDefaultPartitionKey,
                  ensureTableExists: repositoryOptions.Value.EnsureTableExists)
        {
        }
    }
}
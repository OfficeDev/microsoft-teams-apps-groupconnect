// <copyright file="TeamUserPairUpMappingRepository.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Repositories.UserPairupMapping
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos.Table;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Repository of the user pair up data stored in the table storage.
    /// </summary>
    public class TeamUserPairUpMappingRepository : BaseRepository<TeamUserPairUpMappingEntity>
    {
        /// <summary>
        /// Table name for Team user pair up repository.
        /// </summary>
        private const string TeamUserPairUpMappingTableName = "TeamUserPairUpMapping";

        /// <summary>
        /// Default partition key for user pair up repository.
        /// </summary>
        private const string TableDefaultPartitionKey = "TeamId";

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamUserPairUpMappingRepository"/> class.
        /// </summary>
        /// <param name="logger">The logging service.</param>
        /// <param name="repositoryOptions">Options used to create the repository.</param>
        public TeamUserPairUpMappingRepository(
            ILogger<TeamUserPairUpMappingRepository> logger,
            IOptions<RepositoryOptions> repositoryOptions)
            : base(
                  logger,
                  storageAccountConnectionString: repositoryOptions.Value.StorageAccountConnectionString,
                  tableName: TeamUserPairUpMappingTableName,
                  defaultPartitionKey: TableDefaultPartitionKey,
                  ensureTableExists: repositoryOptions.Value.EnsureTableExists)
        {
        }

        /// <summary>
        /// Get active user pair mapping entities from the table storage based on paused flag and rowkey.
        /// </summary>
        /// <param name="rowKey">Row key value.</param>
        /// <returns>List of active user pair mapping entities based on paused flag and rowkey.</returns>
        public async Task<IEnumerable<TeamUserPairUpMappingEntity>> GetActivePairUpUsersAsync(string rowKey)
        {
            try
            {
                string isPausedCondition = TableQuery.GenerateFilterConditionForBool("IsPaused", QueryComparisons.Equal, false);
                var entities = await this.GetRowKeyFilter(rowKey, isPausedCondition);

                return entities;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
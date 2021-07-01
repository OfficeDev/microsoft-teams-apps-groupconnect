// <copyright file="ResourceDataRepository.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Repositories.ResourceData
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos.Table;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.DIConnect.DIConnect.Common.ResourceData;

    /// <summary>
    /// Repository to store resources in the table storage.
    /// </summary>
    public class ResourceDataRepository : BaseRepository<ResourceEntity>, IResourceDataRepository
    {
        /// <summary>
        /// Resource table name.
        /// </summary>
        private const string ResourceTableName = "Resources";

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceDataRepository"/> class.
        /// </summary>
        /// <param name="logger">The logging service.</param>
        /// <param name="repositoryOptions">Options used to create the repository.</param>
        public ResourceDataRepository(
            ILogger<ResourceDataRepository> logger,
            IOptions<RepositoryOptions> repositoryOptions)
            : base(
                  logger,
                  storageAccountConnectionString: repositoryOptions.Value.StorageAccountConnectionString,
                  tableName: ResourceTableName,
                  defaultPartitionKey: Constants.ResourceTablePartitionKey,
                  ensureTableExists: repositoryOptions.Value.EnsureTableExists)
        {
        }

        /// <summary>
        /// Get resource data entities by redirection url or title from the table storage.
        /// </summary>
        /// <param name="redirectionUrl">Resource redirection url.</param>
        /// <param name="title">Resource title.</param>
        /// <returns>Filtered data entities.</returns>
        public async Task<IEnumerable<ResourceEntity>> FindByRedirectionUrlOrTitleAsync(string redirectionUrl, string title)
        {
            string redirectionUrlCondition = TableQuery.GenerateFilterCondition("RedirectionUrl", QueryComparisons.Equal, redirectionUrl);
            string titleCondition = TableQuery.GenerateFilterCondition("ResourceTitle", QueryComparisons.Equal, title);
            string condition = TableQuery.CombineFilters(redirectionUrlCondition, TableOperators.Or, titleCondition);
            var entities = await this.GetWithFilterAsync(condition);

            return entities;
        }
    }
}
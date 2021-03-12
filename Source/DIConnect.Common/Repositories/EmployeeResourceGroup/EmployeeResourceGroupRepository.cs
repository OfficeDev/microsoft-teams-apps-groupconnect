// <copyright file="EmployeeResourceGroupRepository.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Repositories.EmployeeResourceGroup
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos.Table;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Repository of the employee resource group data stored in the table storage.
    /// </summary>
    public class EmployeeResourceGroupRepository : BaseRepository<EmployeeResourceGroupEntity>
    {
        /// <summary>
        /// Table name for employee resource group.
        /// </summary>
        private const string EmployeeResourceGroupTableName = "EmployeeResourceGroup";

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeResourceGroupRepository"/> class.
        /// </summary>
        /// <param name="logger">The logging service.</param>
        /// <param name="repositoryOptions">Options used to create the repository.</param>
        public EmployeeResourceGroupRepository(
            ILogger<EmployeeResourceGroupRepository> logger,
            IOptions<RepositoryOptions> repositoryOptions)
            : base(
                  logger,
                  storageAccountConnectionString: repositoryOptions.Value.StorageAccountConnectionString,
                  tableName: EmployeeResourceGroupTableName,
                  defaultPartitionKey: Constants.ResourceGroupTablePartitionKey,
                  ensureTableExists: repositoryOptions.Value.EnsureTableExists)
        {
        }

        /// <summary>
        /// Get filtered data entities by group link or group name from the table storage.
        /// </summary>
        /// <param name="groupLink">Resource group link.</param>
        /// <param name="groupName">Resource group name.</param>
        /// <returns>Filtered data entities.</returns>
        public async Task<IEnumerable<EmployeeResourceGroupEntity>> GetFilterDataByGroupLinkOrGroupNameAsync(string groupLink, string groupName)
        {
            string groupLinkCondition = TableQuery.GenerateFilterCondition("GroupLink", QueryComparisons.Equal, groupLink);
            string groupNameCondition = TableQuery.GenerateFilterCondition("GroupName", QueryComparisons.Equal, groupName);
            string condition = TableQuery.CombineFilters(groupNameCondition, TableOperators.Or, groupLinkCondition);
            var entities = await this.GetWithFilterAsync(condition);

            return entities;
        }

        /// <summary>
        /// Get employee resource group entity based on team id.
        /// </summary>
        /// <param name="teamId">Resource group team id (19:xxx).</param>
        /// <returns>Employee resource group Entity.</returns>
        public async Task<EmployeeResourceGroupEntity> GetResourceGroupByTeamIdAsync(string teamId)
        {
            string teamIdCondition = TableQuery.GenerateFilterCondition("TeamId", QueryComparisons.Equal, teamId);
            var entities = await this.GetWithFilterAsync(teamIdCondition);

            return entities.FirstOrDefault();
        }

        /// <summary>
        /// Get searchable resource group entities from the table storage.
        /// </summary>
        /// <returns>Returns list of resource groups which are included in search result.</returns>
        public async Task<IEnumerable<EmployeeResourceGroupEntity>> GetSearchableResourceGroupsAsync()
        {
            string includeInSearchResultCondition = TableQuery.GenerateFilterConditionForBool("IncludeInSearchResults", QueryComparisons.Equal, true);
            var entities = await this.GetWithFilterAsync(includeInSearchResultCondition);

            return entities;
        }

        /// <summary>
        /// Get all data resource group entities from the table storage
        /// based on profile matching enabled status and matching frequency.
        /// </summary>
        /// <param name="matchingFrequency">Matching frequency.</param>
        /// <returns>All active profile matching resource group entities based on matching frequency.</returns>
        public async Task<IEnumerable<EmployeeResourceGroupEntity>> GetResourceGroupsOptedForPairUpMatchesAsync(int matchingFrequency)
        {
            string groupTypeCondition = TableQuery.GenerateFilterConditionForInt("GroupType", QueryComparisons.Equal, (int)ResourceGroupType.Teams);
            var profileMatchingCondition = TableQuery.GenerateFilterConditionForBool("IsProfileMatchingEnabled", QueryComparisons.Equal, true);
            var matchingFrequencyCondition = TableQuery.GenerateFilterConditionForInt("MatchingFrequency", QueryComparisons.Equal, matchingFrequency);
            var filter = TableQuery.CombineFilters(profileMatchingCondition, TableOperators.And, matchingFrequencyCondition);
            filter = TableQuery.CombineFilters(filter, TableOperators.And, groupTypeCondition);
            var entities = await this.GetWithFilterAsync(filter);

            return entities;
        }

        /// <summary>
        /// Get all resource group entities from the table storage
        /// based on group type.
        /// </summary>
        /// <param name="groupType">Resource group type.</param>
        /// <returns>All resource groups based on group type.</returns>
        public async Task<IEnumerable<EmployeeResourceGroupEntity>> GetResourceGroupsByTypeAsync(int groupType)
        {
            string groupTypeCondition = TableQuery.GenerateFilterConditionForInt("GroupType", QueryComparisons.Equal, groupType);
            var entities = await this.GetWithFilterAsync(groupTypeCondition);

            return entities;
        }
    }
}
// <copyright file="IEmployeeResourceGroupRepository.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Repositories.EmployeeResourceGroup
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for employee resource group repository.
    /// </summary>
    public interface IEmployeeResourceGroupRepository : IRepository<EmployeeResourceGroupEntity>
    {
        /// <summary>
        /// Get filtered data entities by group link or group name from the table storage.
        /// </summary>
        /// <param name="groupLink">Resource group link.</param>
        /// <param name="groupName">Resource group name.</param>
        /// <returns>Filtered data entities.</returns>
        public Task<IEnumerable<EmployeeResourceGroupEntity>> GetFilterDataByGroupLinkOrGroupNameAsync(string groupLink, string groupName);

        /// <summary>
        /// Get employee resource group entity based on team id.
        /// </summary>
        /// <param name="teamId">Resource group team id (19:xxx).</param>
        /// <returns>Employee resource group Entity.</returns>
        public Task<EmployeeResourceGroupEntity> GetResourceGroupByTeamIdAsync(string teamId);

        /// <summary>
        /// Get searchable resource group entities from the table storage.
        /// </summary>
        /// <returns>Returns list of resource groups which are included in search result.</returns>
        public Task<IEnumerable<EmployeeResourceGroupEntity>> GetSearchableResourceGroupsAsync();

        /// <summary>
        /// Get all data resource group entities from the table storage
        /// based on profile matching enabled status and matching frequency.
        /// </summary>
        /// <param name="matchingFrequency">Matching frequency.</param>
        /// <returns>All active profile matching resource group entities based on matching frequency.</returns>
        public Task<IEnumerable<EmployeeResourceGroupEntity>> GetResourceGroupsOptedForPairUpMatchesAsync(int matchingFrequency);

        /// <summary>
        /// Get all resource group entities from the table storage
        /// based on group type.
        /// </summary>
        /// <param name="groupType">Resource group type.</param>
        /// <returns>All resource groups based on group type.</returns>
        public Task<IEnumerable<EmployeeResourceGroupEntity>> GetResourceGroupsByTypeAsync(int groupType);
    }
}
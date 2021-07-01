// <copyright file="ITeamUserPairUpMappingRepository.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Repositories.UserPairupMapping
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for user pair up mapping.
    /// </summary>
    public interface ITeamUserPairUpMappingRepository : IRepository<TeamUserPairUpMappingEntity>
    {
        /// <summary>
        /// Get active user pair mapping entities from the table storage based on paused flag and rowkey.
        /// </summary>
        /// <param name="rowKey">Row key value.</param>
        /// <returns>List of active user pair mapping entities based on paused flag and rowkey.</returns>
        public Task<IEnumerable<TeamUserPairUpMappingEntity>> GetActivePairUpUsersAsync(string rowKey);
    }
}
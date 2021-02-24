// <copyright file="IGroupsService.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Services.MicrosoftGraph
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Graph;

    /// <summary>
    /// Interface for Groups Service.
    /// </summary>
    public interface IGroupsService
    {
        /// <summary>
        /// get the group by ids.
        /// </summary>
        /// <param name="groupIds">list of group ids.</param>
        /// <returns>list of groups.</returns>
        IAsyncEnumerable<Group> GetByIdsAsync(IEnumerable<string> groupIds);

        /// <summary>
        /// check if list has hidden membership group.
        /// </summary>
        /// <param name="groupIds">list of group ids.</param>
        /// <returns>boolean.</returns>
        Task<bool> ContainsHiddenMembershipAsync(IEnumerable<string> groupIds);

        /// <summary>
        /// Search groups based on query.
        /// </summary>
        /// <param name="query">Query param.</param>
        /// <returns>list of group.</returns>
        Task<IList<Group>> SearchAsync(string query);

        /// <summary>
        /// Get team details based on group id.
        /// </summary>
        /// <param name="groupId">Group id of the team.</param>
        /// <returns>Team details.</returns>
        Task<Group> GetTeamsInfoAsync(string groupId);

        /// <summary>
        /// Get team owners Azure active directory object id's based on group id.
        /// </summary>
        /// <param name="groupId">Group id of the team.</param>
        /// <returns>Team owners Azure active directory object id's.</returns>
        Task<ISet<string>> GetTeamOwnersAadObjectIdAsync(string groupId);
    }
}
// <copyright file="IGroupMembersService.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Services.MicrosoftGraph
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Graph;

    /// <summary>
    /// Interface for Group Members Service.
    /// </summary>
    public interface IGroupMembersService
    {
        /// <summary>
        /// Get groups members.
        /// </summary>
        /// <param name="groupId">Group Id.</param>
        /// <returns>Enumerator to iterate over a collection of <see cref="User"/>.</returns>
        Task<IEnumerable<User>> GetGroupMembersAsync(string groupId);

        /// <summary>
        /// get group members page by id.
        /// </summary>
        /// <param name="groupId">group id.</param>
        /// <returns>group members page.</returns>
        Task<IGroupTransitiveMembersCollectionWithReferencesPage> GetGroupMembersPageByIdAsync(string groupId);

        /// <summary>
        /// get group members page by next page URL.
        /// </summary>
        /// <param name="groupMembersRef">group members page reference.</param>
        /// <param name="nextPageUrl">group members next page data link URL.</param>
        /// <returns>group members page.</returns>
        Task<IGroupTransitiveMembersCollectionWithReferencesPage> GetGroupMembersNextPageAsnyc(IGroupTransitiveMembersCollectionWithReferencesPage groupMembersRef, string nextPageUrl);
    }
}
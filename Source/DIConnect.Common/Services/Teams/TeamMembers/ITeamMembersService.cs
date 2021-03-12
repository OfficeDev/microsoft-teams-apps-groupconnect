// <copyright file="ITeamMembersService.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Services.Teams
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.UserData;

    /// <summary>
    /// Team Members service interface.
    /// </summary>
    public interface ITeamMembersService
    {
        /// <summary>
        /// Get all the members in a team.
        /// </summary>
        /// <param name="teamId">Team Id. Example: "19:44777361677b439281a0f0cd914cb149@thread.skype".</param>
        /// <param name="tenantId">Tenant Id.</param>
        /// <param name="serviceUrl">Service url.</param>
        /// <returns>All the members in a team.</returns>
        public Task<IEnumerable<UserDataEntity>> GetMembersAsync(string teamId, string tenantId, string serviceUrl);
    }
}
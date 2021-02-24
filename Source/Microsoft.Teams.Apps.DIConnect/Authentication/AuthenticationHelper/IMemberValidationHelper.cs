// <copyright file="IMemberValidationHelper.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Authentication.AuthenticationHelper
{
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for member validation.
    /// </summary>
    public interface IMemberValidationHelper
    {
        /// <summary>
        /// Check if a user is a member of a admin team.
        /// </summary>
        /// <param name="userAadObjectId">The user's Azure Active Directory object id.</param>
        /// <returns>The flag indicates that the user is a part of certain team or not.</returns>
        Task<bool> IsAdminTeamMemberAsync(string userAadObjectId);
    }
}
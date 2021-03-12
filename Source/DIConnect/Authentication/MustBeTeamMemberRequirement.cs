// <copyright file="MustBeTeamMemberRequirement.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Authentication
{
    using Microsoft.AspNetCore.Authorization;

    /// <summary>
    /// This class is an authorization policy requirement.
    /// It specifies that an id token must contain valid team member claim.
    /// </summary>
    public class MustBeTeamMemberRequirement : IAuthorizationRequirement
    {
    }
}
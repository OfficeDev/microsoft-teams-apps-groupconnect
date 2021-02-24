// <copyright file="MustBeTeamOwnerOrAdminUserHandlerRequirement.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Authentication
{
    using Microsoft.AspNetCore.Authorization;

    /// <summary>
    /// This class is an authorization policy requirement.
    /// It specifies that an id token must contain valid team owner claim.
    /// </summary>
    public class MustBeTeamOwnerOrAdminUserHandlerRequirement : IAuthorizationRequirement
    {
    }
}
// <copyright file="MSGraphScopeRequirement.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Authentication
{
    using Microsoft.AspNetCore.Authorization;

    /// <summary>
    /// This class is an authorization policy requirement.
    /// It specifies that an access token must contain group.read.all scope.
    /// </summary>
    public class MSGraphScopeRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MSGraphScopeRequirement"/> class.
        /// </summary>
        /// <param name="scopes">Microsoft Graph Scopes.</param>
       public MSGraphScopeRequirement(string[] scopes)
        {
            this.Scopes = scopes;
        }

        /// <summary>
        /// Gets Microsoft Graph Scopes.
        /// </summary>
       public string[] Scopes { get; private set; }
    }
}
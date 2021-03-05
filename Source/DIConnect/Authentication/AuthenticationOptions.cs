// <copyright file="AuthenticationOptions.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Authentication
{
    /// <summary>
    /// Options used for setting up the authentication.
    /// </summary>
    public class AuthenticationOptions
    {
        /// <summary>
        /// Gets or sets the Azure active directory instance.
        /// </summary>
        public string AzureAdInstance { get; set; }

        /// <summary>
        /// Gets or sets the Azure active directory tenant id.
        /// </summary>
        public string AzureAdTenantId { get; set; }

        /// <summary>
        /// Gets or sets the Azure active directory client id.
        /// </summary>
        public string AzureAdClientId { get; set; }

        /// <summary>
        /// Gets or sets the Azure active directory application id URI.
        /// </summary>
        public string AzureAdApplicationIdUri { get; set; }

        /// <summary>
        /// Gets or sets the Azure active directory valid issuers.
        /// </summary>
        public string AzureAdValidIssuers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the "must be a UPN in the authorized list
        /// in order to use the app and create notifications" check should be disabled.
        /// </summary>
        public bool DisableCreatorUpnCheck { get; set; }

        /// <summary>
        /// Gets or sets admin team id. Members of the team will be allowed to access the app and use it to
        /// create notification and can manage ERG groups profile matching configurations.
        /// </summary>
        public string AdminTeamId { get; set; }
    }
}
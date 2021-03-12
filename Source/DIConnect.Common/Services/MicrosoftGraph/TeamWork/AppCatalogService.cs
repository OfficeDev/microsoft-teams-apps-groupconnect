// <copyright file="AppCatalogService.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Services.MicrosoftGraph
{
    extern alias BetaLib;

    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using BetaLib::Microsoft.Graph;

    /// <summary>
    /// Read information about the apps published in the Teams app store and organization's app catalog.
    /// </summary>
    internal class AppCatalogService : IAppCatalogService
    {
        private readonly IGraphServiceClient betaServiceClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppCatalogService"/> class.
        /// </summary>
        /// <param name="betaServiceClient">Beta Graph service client.</param>
        internal AppCatalogService(IGraphServiceClient betaServiceClient)
        {
            this.betaServiceClient = betaServiceClient ?? throw new ArgumentNullException(nameof(betaServiceClient));
        }

        /// <inheritdoc/>
        public async Task<string> GetTeamsAppIdAsync(string externalId)
        {
            if (externalId == null)
            {
                throw new ArgumentNullException(nameof(externalId));
            }

            var apps = await this.betaServiceClient
                .AppCatalogs
                .TeamsApps
                .Request()
                .Header(Common.Constants.PermissionTypeKey, GraphPermissionType.Delegate.ToString())
                .Filter($"distributionMethod eq 'organization' and externalId eq '{externalId}'")
                .GetAsync();

            return apps?.FirstOrDefault()?.Id;
        }
    }
}
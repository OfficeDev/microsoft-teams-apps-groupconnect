// <copyright file="IAppSettingsService.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Services
{
    using System.Threading.Tasks;

    /// <summary>
    /// App settings interface.
    /// </summary>
    public interface IAppSettingsService
    {
        /// <summary>
        /// Gets cached user app id.
        /// </summary>
        /// <returns>User app id.</returns>
        public Task<string> GetUserAppIdAsync();

        /// <summary>
        /// Gets cached service url.
        /// </summary>
        /// <returns>Service url.</returns>
        public Task<string> GetServiceUrlAsync();

        /// <summary>
        /// Persists the user app id in database.
        /// </summary>
        /// <param name="userAppId">User app id.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task SetUserAppIdAsync(string userAppId);

        /// <summary>
        /// Persists the service url in database.
        /// </summary>
        /// <param name="serviceUrl">Service url.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task SetServiceUrlAsync(string serviceUrl);
    }
}
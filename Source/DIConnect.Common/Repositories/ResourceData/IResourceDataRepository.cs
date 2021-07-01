// <copyright file="IResourceDataRepository.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Repositories.ResourceData
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Teams.Apps.DIConnect.DIConnect.Common.ResourceData;

    /// <summary>
    /// Interface for resource data repository.
    /// </summary>
    public interface IResourceDataRepository : IRepository<ResourceEntity>
    {
        /// <summary>
        /// Get resource entities by redirection url or title from the table storage.
        /// </summary>
        /// <param name="redirectionUrl">Resource redirection url.</param>
        /// <param name="title">Resource title.</param>
        /// <returns>Filtered data entities.</returns>
        public Task<IEnumerable<ResourceEntity>> FindByRedirectionUrlOrTitleAsync(string redirectionUrl, string title);
    }
}
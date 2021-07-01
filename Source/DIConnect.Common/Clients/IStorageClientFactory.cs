// <copyright file="IStorageClientFactory.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Clients
{
    using global::Azure.Storage.Blobs;

    /// <summary>
    /// Storage client factory.
    /// </summary>
    public interface IStorageClientFactory
    {
        /// <summary>
        /// Create the blob container client instance.
        /// </summary>
        /// <returns>BlobContainerClient instance.</returns>
        BlobContainerClient CreateBlobContainerClient();
    }
}
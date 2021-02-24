// <copyright file="UsersSyncEntity.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Repositories
{
    using Microsoft.Azure.Cosmos.Table;

    /// <summary>
    /// Users sync information.
    /// </summary>
    public class UsersSyncEntity : TableEntity
    {
        /// <summary>
        /// Gets or sets the entity value.
        /// </summary>
        public string Value { get; set; }
    }
}
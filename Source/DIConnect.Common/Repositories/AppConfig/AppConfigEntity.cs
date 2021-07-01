// <copyright file="AppConfigEntity.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Repositories
{
    using Microsoft.Azure.Cosmos.Table;

    /// <summary>
    /// App configuration entity.
    /// </summary>
    public class AppConfigEntity : TableEntity
    {
        /// <summary>
        /// Gets or sets the entity value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether configuration is enabled or not.
        /// </summary>
        public bool IsEnabled { get; set; }
    }
}
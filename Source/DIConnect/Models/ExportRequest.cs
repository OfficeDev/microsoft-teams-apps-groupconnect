// <copyright file="ExportRequest.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Models
{
    /// <summary>
    /// Export request model class.
    /// </summary>
    public class ExportRequest
    {
        /// <summary>
        /// Gets or sets the notification id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the Team Id.
        /// </summary>
        public string TeamId { get; set; }
    }
}
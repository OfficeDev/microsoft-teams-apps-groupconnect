// <copyright file="DraftNotificationPreviewRequest.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Models
{
    /// <summary>
    /// Draft notification preview request model class.
    /// </summary>
    public class DraftNotificationPreviewRequest
    {
        /// <summary>
        /// Gets or sets draft notification id.
        /// </summary>
        public string DraftNotificationId { get; set; }

        /// <summary>
        /// Gets or sets Teams team id.
        /// </summary>
        public string TeamsTeamId { get; set; }

        /// <summary>
        /// Gets or sets Teams channel id.
        /// </summary>
        public string TeamsChannelId { get; set; }
    }
}
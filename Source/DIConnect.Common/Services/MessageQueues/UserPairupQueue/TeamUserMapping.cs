// <copyright file="TeamUserMapping.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Services.MessageQueues.UserPairupQueue
{
    /// <summary>
    /// User and Team mapping data object for message content.
    /// </summary>
    public class TeamUserMapping : UserData
    {
        /// <summary>
        /// Gets or sets the Team id.
        /// </summary>
        public string TeamId { get; set; }

        /// <summary>
        /// Gets or sets the Team name.
        /// </summary>
        public string TeamName { get; set; }
    }
}
// <copyright file="SubmitActionDataForTeamsBehavior.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// Defines teams-specific behavior for an card submit action.
    /// </summary>
    public class SubmitActionDataForTeamsBehavior
    {
        /// <summary>
        /// Gets or sets command text to show submit or cancel event on Task Module.
        /// </summary>
        [JsonProperty("command")]
        public string Command { get; set; }

        /// <summary>
        /// Gets or sets feedback description from user.
        /// </summary>
        [JsonProperty("FeedbackDescription")]
        public string FeedbackDescription { get; set; }

        /// <summary>
        /// Gets or sets feedback type from user.
        /// </summary>
        [JsonProperty("FeedbackType")]
        public string FeedbackType { get; set; }

        /// <summary>
        /// Gets or sets the user name who created the resource group.
        /// </summary>
        [JsonProperty("createdByName")]
        public string CreatedByName { get; set; }

        /// <summary>
        /// Gets or sets employee resource group id.
        /// </summary>
        [JsonProperty("groupId")]
        public string GroupId { get; set; }

        /// <summary>
        /// Gets or sets team id.
        /// </summary>
        [JsonProperty("teamId")]
        public string TeamId { get; set; }
    }
}
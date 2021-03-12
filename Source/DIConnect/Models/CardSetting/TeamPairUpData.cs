// <copyright file="TeamPairUpData.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Models.CardSetting
{
    using Newtonsoft.Json;

    /// <summary>
    /// Team pair-up data class.
    /// This entity holds the content required for user team pair-up matches notification card.
    /// </summary>
    public class TeamPairUpData
    {
        /// <summary>
        /// Gets or sets team display name.
        /// </summary>
        [JsonProperty("title")]
        public string TeamDisplayName { get; set; }

        /// <summary>
        /// Gets or sets team id value.
        /// </summary>
        [JsonProperty("value")]
        public string TeamId { get; set; }
    }
}
// <copyright file="ResumePairupMatchesCardData.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Models.CardSetting
{
    using Newtonsoft.Json;

    /// <summary>
    /// Resume pair-up matches card data class.
    /// This entity holds the content required for resume pair-up matches notification card.
    /// </summary>
    public class ResumePairupMatchesCardData
    {
        /// <summary>
        /// Gets or sets update user matches title text value.
        /// </summary>
        [JsonProperty("updateCardTitle")]
        public string UpdateCardTitle { get; set; }

        /// <summary>
        /// Gets or sets update user matches button text value.
        /// </summary>
        [JsonProperty("updateCardButtonText")]
        public string UpdateCardButtonText { get; set; }

        /// <summary>
        /// Gets or sets configure matches command.
        /// </summary>
        [JsonProperty("configureMatchesCommand")]
        public string ConfigureMatchesCommand { get; set; }
    }
}
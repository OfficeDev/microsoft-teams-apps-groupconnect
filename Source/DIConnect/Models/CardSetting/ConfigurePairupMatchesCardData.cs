// <copyright file="ConfigurePairupMatchesCardData.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Models.CardSetting
{
    using Newtonsoft.Json;

    /// <summary>
    /// Configure pair-up matches card data class.
    /// This entity holds the content required for configure pair-up matches card.
    /// </summary>
    public class ConfigurePairupMatchesCardData
    {
        /// <summary>
        /// Gets or sets configure matches title text value.
        /// </summary>
        [JsonProperty("configureMatchesCardTitle")]
        public string ConfigureMatchesCardTitle { get; set; }

        /// <summary>
        /// Gets or sets configure matches button text value.
        /// </summary>
        [JsonProperty("configureMatchesButtonText")]
        public string ConfigureMatchesButtonText { get; set; }

        /// <summary>
        /// Gets or sets configure matches command.
        /// </summary>
        [JsonProperty("configureMatchesCommand")]
        public string ConfigureMatchesCommand { get; set; }
    }
}
// <copyright file="UserPairUpMatchesNotificationCardData.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Models.CardSetting
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// User pair up matches notification card data class.
    /// This entity holds the content required for user pair-up matches notification card.
    /// </summary>
    public class UserPairUpMatchesNotificationCardData
    {
        /// <summary>
        /// Gets or sets configure user matches card title text value.
        /// </summary>
        [JsonProperty("configureUserMatchesCardTitleText")]
        public string ConfigureUserMatchesCardTitleText { get; set; }

        /// <summary>
        /// Gets or sets a collection of team pair-up entities.
        /// </summary>
        [JsonProperty("teamPairUpEntities")]
        public IEnumerable<TeamPairUpData> TeamPairUpEntities { get; set; }

        /// <summary>
        /// Gets or sets comma separated team id's.
        /// </summary>
        [JsonProperty("teamIds")]
        public string CommaSeparatedTeamIds { get; set; }

        /// <summary>
        /// Gets or sets configure user matches card title text value.
        /// </summary>
        [JsonProperty("configureUserMatchesButtonText")]
        public string ConfigureUserMatchesButtonText { get; set; }

        /// <summary>
        /// Gets or sets update matches command.
        /// </summary>
        [JsonProperty("updateMatchesCommand")]
        public string UpdateMatchesCommand { get; set; }
    }
}
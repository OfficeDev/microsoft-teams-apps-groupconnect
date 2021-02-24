// <copyright file="PairUpNotificationCardData.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Send.Func.Cards
{
    using Newtonsoft.Json;

    /// <summary>
    /// Pair up notification card data class.
    /// This entity holds the content required for pair up notification card.
    /// </summary>
    public class PairUpNotificationCardData
    {
        /// <summary>
        /// Gets or sets match up title text value.
        /// </summary>
        [JsonProperty("matchUpCardTitleText")]
        public string MatchUpCardTitleText { get; set; }

        /// <summary>
        /// Gets or sets match up card sub header text value.
        /// </summary>
        [JsonProperty("matchUpCardSubHeaderText")]
        public string MatchUpCardSubHeaderText { get; set; }

        /// <summary>
        /// Gets or sets match up card content value.
        /// </summary>
        [JsonProperty("matchUpCardContent")]
        public string MatchUpCardContent { get; set; }

        /// <summary>
        /// Gets or sets chat with user button text value.
        /// </summary>
        [JsonProperty("chatWithUserButtonText")]
        public string ChatWithUserButtonText { get; set; }

        /// <summary>
        /// Gets or sets chat initiate url text value.
        /// </summary>
        [JsonProperty("chatInitiateURL")]
        public string ChatInitiateURL { get; set; }

        /// <summary>
        /// Gets or sets propose meet up button text value.
        /// </summary>
        [JsonProperty("proposeMeetupButtonText")]
        public string ProposeMeetupButtonText { get; set; }

        /// <summary>
        /// Gets or sets meeting link value.
        /// </summary>
        [JsonProperty("meetingLink")]
        public string MeetingLink { get; set; }

        /// <summary>
        /// Gets or sets pause matches button text value.
        /// </summary>
        [JsonProperty("pauseMatchesButtonText")]
        public string PauseMatchesButtonText { get; set; }

        /// <summary>
        /// Gets or sets pause matches display text value.
        /// </summary>
        [JsonProperty("pauseMatchesText")]
        public string PauseMatchesText { get; set; }

        /// <summary>
        /// Gets or sets team id value.
        /// </summary>
        [JsonProperty("teamId")]
        public string TeamId { get; set; }
    }
}
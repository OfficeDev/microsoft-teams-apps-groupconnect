// <copyright file="ShareFeedbackCardData.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Models.CardSetting
{
    using Newtonsoft.Json;

    /// <summary>
    /// Share feedback card data class.
    /// This entity holds the content required for share feedback card.
    /// </summary>
    public class ShareFeedbackCardData
    {
        /// <summary>
        /// Gets or sets feedback text value.
        /// </summary>
        [JsonProperty("feedbackText")]
        public string FeedbackText { get; set; }

        /// <summary>
        /// Gets or sets feedback sub header text value.
        /// </summary>
        [JsonProperty("feedbackSubHeaderText")]
        public string FeedbackSubHeaderText { get; set; }

        /// <summary>
        /// Gets or sets feedback type value.
        /// </summary>
        [JsonProperty("feedbackType")]
        public string FeedbackType { get; set; }

        /// <summary>
        /// Gets or sets description text value.
        /// </summary>
        [JsonProperty("descriptionText")]
        public string DescriptionText { get; set; }

        /// <summary>
        /// Gets or sets feedback description value.
        /// </summary>
        [JsonProperty("feedbackDescription")]
        public string FeedbackDescription { get; set; }

        /// <summary>
        /// Gets or sets created on text value.
        /// </summary>
        [JsonProperty("createdOnText")]
        public string CreatedOnText { get; set; }

        /// <summary>
        /// Gets or sets feedback created date value.
        /// </summary>
        [JsonProperty("feedbackCreatedDate")]
        public string FeedbackCreatedDate { get; set; }

        /// <summary>
        /// Gets or sets chat initiate url text value.
        /// </summary>
        [JsonProperty("chatInitiateURL")]
        public string ChatInitiateURL { get; set; }

        /// <summary>
        /// Gets or sets chat with user button text value.
        /// </summary>
        [JsonProperty("chatWithUserButtonText")]
        public string ChatWithUserButtonText { get; set; }
    }
}
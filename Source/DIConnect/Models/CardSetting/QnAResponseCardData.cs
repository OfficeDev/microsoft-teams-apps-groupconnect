// <copyright file="QnAResponseCardData.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Models.CardSetting
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// QnA response card data class.
    /// This entity holds the content required for QnA response card.
    /// </summary>
    public class QnAResponseCardData
    {
        /// <summary>
        /// Gets or sets response header text value.
        /// </summary>
        [JsonProperty("responseHeaderText")]
        public string ResponseHeaderText { get; set; }

        /// <summary>
        /// Gets or sets question text value.
        /// </summary>
        [JsonProperty("questionText")]
        public string QuestionText { get; set; }

        /// <summary>
        /// Gets or sets answer text value.
        /// </summary>
        [JsonProperty("answerText")]
        public string AnswerText { get; set; }

        /// <summary>
        /// Gets or sets share feedback button text value.
        /// </summary>
        [JsonProperty("shareFeedbackButtonText")]
        public string ShareFeedbackButtonText { get; set; }

        /// <summary>
        /// Gets or sets feedback header text value.
        /// </summary>
        [JsonProperty("feedbackHeaderText")]
        public string FeedbackHeaderText { get; set; }

        /// <summary>
        /// Gets or sets feedback title text value.
        /// </summary>
        [JsonProperty("feedbackTitleText")]
        public string FeedbackTitleText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is prompt questions present or not.
        /// </summary>
        [JsonProperty("isPromptQuestionsPresent")]
        public bool IsPromptQuestionsPresent { get; set; }

        /// <summary>
        /// Gets or sets prompt header text value.
        /// </summary>
        [JsonProperty("promptHeaderText")]
        public string PromptHeaderText { get; set; }

        /// <summary>
        /// Gets or sets helpful title text value.
        /// </summary>
        [JsonProperty("helpfulTitleText")]
        public string HelpfulTitleText { get; set; }

        /// <summary>
        /// Gets or sets needs improvement title text value.
        /// </summary>
        [JsonProperty("needsImprovementTitleText")]
        public string NeedsImprovementTitleText { get; set; }

        /// <summary>
        /// Gets or sets not helpful title text value.
        /// </summary>
        [JsonProperty("notHelpfulTitleText")]
        public string NotHelpfulTitleText { get; set; }

        /// <summary>
        /// Gets or sets choice set place holder text value.
        /// </summary>
        [JsonProperty("choiceSetPlaceholder")]
        public string ChoiceSetPlaceholder { get; set; }

        /// <summary>
        /// Gets or sets description text value.
        /// </summary>
        [JsonProperty("descriptionText")]
        public string DescriptionText { get; set; }

        /// <summary>
        /// Gets or sets description placeholder text value.
        /// </summary>
        [JsonProperty("descriptionPlaceHolderText")]
        public string DescriptionPlaceHolderText { get; set; }

        /// <summary>
        /// Gets or sets share button text value.
        /// </summary>
        [JsonProperty("shareButtonText")]
        public string ShareButtonText { get; set; }

        /// <summary>
        /// Gets or sets share command value.
        /// </summary>
        [JsonProperty("shareCommand")]
        public string ShareCommand { get; set; }

        /// <summary>
        /// Gets or sets list of column set elements.
        /// </summary>
        [JsonProperty("columnSets")]
        public List<ColumnData> ColumnSets { get; set; }
    }
}
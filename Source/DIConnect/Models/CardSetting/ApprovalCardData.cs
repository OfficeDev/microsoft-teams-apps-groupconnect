// <copyright file="ApprovalCardData.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Models.CardSetting
{
    using Newtonsoft.Json;

    /// <summary>
    /// Approval card data class.
    /// This entity holds the content required for approval card.
    /// </summary>
    public class ApprovalCardData
    {
        /// <summary>
        /// Gets or sets approval card request submitted text value.
        /// </summary>
        [JsonProperty("requestSubmittedText")]
        public string RequestSubmittedText { get; set; }

        /// <summary>
        /// Gets or sets approval card approve/reject text value.
        /// </summary>
        [JsonProperty("approvalStatusText")]
        public string ApprovalStatusText { get; set; }

        /// <summary>
        /// Gets or sets approval status text value.
        /// </summary>
        [JsonProperty("approvalStatus")]
        public string ApprovalStatus { get; set; }

        /// <summary>
        /// Gets or sets approval card group description text value.
        /// </summary>
        [JsonProperty("groupDescriptionText")]
        public string GroupDescriptionText { get; set; }

        /// <summary>
        /// Gets or sets approval card name label text value.
        /// </summary>
        [JsonProperty("nameText")]
        public string NameText { get; set; }

        /// <summary>
        /// Gets or sets approval card group name text value.
        /// </summary>
        [JsonProperty("groupNameText")]
        public string GroupNameText { get; set; }

        /// <summary>
        /// Gets or sets approval card tags text value.
        /// </summary>
        [JsonProperty("tagsText")]
        public string TagsText { get; set; }

        /// <summary>
        /// Gets or sets approval card tags name value.
        /// </summary>
        [JsonProperty("tagsName")]
        public string ApproveTagsName { get; set; }

        /// <summary>
        /// Gets or sets approval card Location text value.
        /// </summary>
        [JsonProperty("locationText")]
        public string LocationText { get; set; }

        /// <summary>
        /// Gets or sets approval card Location name value.
        /// </summary>
        [JsonProperty("locationName")]
        public string LocationName { get; set; }

        /// <summary>
        /// Gets or sets approval card created by name text value.
        /// </summary>
        [JsonProperty("createdByNameText")]
        public string CreatedByNameText { get; set; }

        /// <summary>
        /// Gets or sets approval card created by user name value.
        /// </summary>
        [JsonProperty("createdByName")]
        public string CreatedByName { get; set; }

        /// <summary>
        /// Gets or sets approval card Search enable text value.
        /// </summary>
        [JsonProperty("searchEnableText")]
        public string SearchEnableText { get; set; }

        /// <summary>
        /// Gets or sets approval card Search enable status text value.
        /// </summary>
        [JsonProperty("searchEnableStatusText")]
        public string SearchEnableStatusText { get; set; }

        /// <summary>
        /// Gets or sets approval card approve text value.
        /// </summary>
        [JsonProperty("approveButtonText")]
        public string ApproveButtonText { get; set; }

        /// <summary>
        /// Gets or sets approval card reject text value.
        /// </summary>
        [JsonProperty("rejectButtonText")]
        public string RejectButtonText { get; set; }

        /// <summary>
        /// Gets or sets approved command text value.
        /// </summary>
        [JsonProperty("approvedCommandText")]
        public string ApprovedCommandText { get; set; }

        /// <summary>
        /// Gets or sets reject command text value.
        /// </summary>
        [JsonProperty("rejectCommandText")]
        public string RejectCommandText { get; set; }

        /// <summary>
        /// Gets or sets employee resource group id.
        /// </summary>
        [JsonProperty("groupId")]
        public string GroupId { get; set; }
    }
}
// <copyright file="EmployeeResourceGroupEntity.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Repositories.EmployeeResourceGroup
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.Azure.Cosmos.Table;

    /// <summary>
    /// Employee resource group entity class.
    /// This entity holds the information about a employee resource group.
    /// </summary>
    public class EmployeeResourceGroupEntity : TableEntity
    {
        /// <summary>
        /// Gets or sets the group as default partition key.
        /// </summary>
        public string Group
        {
            get { return this.PartitionKey; }
            set { this.PartitionKey = value; }
        }

        /// <summary>
        /// Gets or sets the group type.
        /// A enum that represent the resource group type like Teams == 0, External == 1.
        /// </summary>
        [Required]
        public int GroupType { get; set; }

        /// <summary>
        /// Gets or sets unique identifier for each created group.
        /// </summary>
        [Required]
        public string GroupId
        {
            get { return this.RowKey; }
            set { this.RowKey = value; }
        }

        /// <summary>
        /// Gets or sets the group name.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets the group description.
        /// </summary>
        [Required]
        [MaxLength(300)]
        public string GroupDescription { get; set; }

        /// <summary>
        /// Gets or sets the group link.
        /// </summary>
        [Required]
        [Url]
        public string GroupLink { get; set; }

        /// <summary>
        /// Gets or sets the image link.
        /// </summary>
        [Required]
        [Url]
        public string ImageLink { get; set; }

        /// <summary>
        /// Gets or sets tags entered by user in json format.
        /// </summary>
        [TagsValidationAttribute(3, 20)]
        public string Tags { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether search is enabled or not. If ERG group crated with search enable it will go to admin team for approval.
        /// Only search enabled groups are available to all end users.
        /// </summary>
        public bool IncludeInSearchResults { get; set; }

        /// <summary>
        /// Gets or sets approval status using enum values. Admin team member can Approve/Reject any ERG request to make it searchable.
        /// </summary>
        public int ApprovalStatus { get; set; }

        /// <summary>
        /// Gets or sets matching frequency either weekly/monthly using enum values.
        /// </summary>
        public int MatchingFrequency { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether pair-up profile matching is enabled or not for a given Team.
        /// </summary>
        public bool IsProfileMatchingEnabled { get; set; }

        /// <summary>
        /// Gets or sets the created on date time.
        /// </summary>
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the user AAD object id who created the ERG group.
        /// </summary>
        public string CreatedByObjectId { get; set; }

        /// <summary>
        /// Gets or sets the updated on date time.
        /// </summary>
        public DateTime? UpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets the user AAD object id.
        /// </summary>
        public string UpdatedByObjectId { get; set; }

        /// <summary>
        /// Gets or sets the team id. e.g. 19:xxx.
        /// </summary>
        public string TeamId { get; set; }
    }
}
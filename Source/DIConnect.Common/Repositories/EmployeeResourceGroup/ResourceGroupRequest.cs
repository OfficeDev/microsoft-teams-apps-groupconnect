// <copyright file="ResourceGroupRequest.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Repositories.EmployeeResourceGroup
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Employee resource group entity class.
    /// This entity holds the information about a employee resource group.
    /// </summary>
    public class ResourceGroupRequest
    {
        /// <summary>
        /// Gets or sets the group type.
        /// A enum that represent the resource group type like Teams == 0, External == 1.
        /// </summary>
        [Required]
        public int GroupType { get; set; }

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
        /// Gets or sets semicolon separated tags entered by user.
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
        /// Gets or sets matching frequency either weekly/monthly using enum values.
        /// </summary>
        public int MatchingFrequency { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether pair-up profile matching is enabled or not for a given Team.
        /// </summary>
        public bool IsProfileMatchingEnabled { get; set; }
    }
}
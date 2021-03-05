// <copyright file="ResourceGroupResponse.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Repositories.EmployeeResourceGroup
{
    using System.Collections.Generic;

    /// <summary>
    /// Employee resource group response entity class.
    /// This entity holds the information about a employee resource group response.
    /// </summary>
    public class ResourceGroupResponse
    {
        /// <summary>
        /// Gets or sets the group type.
        /// </summary>
        public int GroupType { get; set; }

        /// <summary>
        /// Gets or sets unique identifier for each created group.
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// Gets or sets the group name.
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets the group description.
        /// </summary>
        public string GroupDescription { get; set; }

        /// <summary>
        /// Gets or sets the group link.
        /// </summary>
        public string GroupLink { get; set; }

        /// <summary>
        /// Gets or sets the image link.
        /// </summary>
        public string ImageLink { get; set; }

        /// <summary>
        /// Gets or sets semicolon separated tags entered by user.
        /// </summary>
        public IList<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
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
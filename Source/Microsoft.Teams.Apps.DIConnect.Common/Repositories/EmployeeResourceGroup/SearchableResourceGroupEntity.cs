// <copyright file="SearchableResourceGroupEntity.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Repositories.EmployeeResourceGroup
{
    /// <summary>
    /// Searchable employee resource group entity for end user discover tab.
    /// This entity holds the information about a employee resource group.
    /// </summary>
    public class SearchableResourceGroupEntity
    {
        /// <summary>
        /// Gets or sets the group Id.
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
        /// Gets or sets JSON formatted tags entered by user.
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        public string Location { get; set; }
    }
}
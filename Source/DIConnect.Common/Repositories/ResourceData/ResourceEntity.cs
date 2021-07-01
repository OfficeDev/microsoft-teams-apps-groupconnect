// <copyright file="ResourceEntity.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.DIConnect.Common.ResourceData
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.Azure.Cosmos.Table;

    /// <summary>
    /// Class contains resource details.
    /// </summary>
    public class ResourceEntity : TableEntity
    {
        /// <summary>
        /// Gets or sets the resource as default partition key.
        /// </summary>
        public string Resource
        {
            get { return this.PartitionKey; }
            set { this.PartitionKey = value; }
        }

        /// <summary>
        /// Gets or sets unique identifier for each created resource.
        /// </summary>
        [Required]
        public string ResourceId
        {
            get { return this.RowKey; }
            set { this.RowKey = value; }
        }

        /// <summary>
        /// Gets or sets the resource title.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string ResourceTitle { get; set; }

        /// <summary>
        /// Gets or sets the resource description.
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string ResourceDescription { get; set; }

        /// <summary>
        /// Gets or sets the resource image link.
        /// </summary>
        [Required]
        [Url]
        public string ImageLink { get; set; }

        /// <summary>
        /// Gets or sets the resource redirection url.
        /// </summary>
        [Required]
        [Url]
        public string RedirectionUrl { get; set; }

        /// <summary>
        /// Gets or sets the resource type.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string ResourceType { get; set; }

        /// <summary>
        /// Gets or sets the created on date time.
        /// </summary>
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the user AAD object id who created the resource.
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
    }
}
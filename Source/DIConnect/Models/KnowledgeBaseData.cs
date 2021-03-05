// <copyright file="KnowledgeBaseData.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Models
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Knowledge base data model class.
    /// </summary>
    public class KnowledgeBaseData
    {
        /// <summary>
        /// Gets or sets knowledge base Id.
        /// </summary>
        [Required]
        public string Id { get; set; }
    }
}
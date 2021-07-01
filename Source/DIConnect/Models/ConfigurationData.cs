// <copyright file="ConfigurationData.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Models
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Configuration data model class.
    /// </summary>
    public class ConfigurationData
    {
        /// <summary>
        /// Gets or sets QnA maker knowledge base Id.
        /// </summary>
        [Required]
        public string QnAMakerKnowledgeBaseId { get; set; }

        /// <summary>
        /// Gets or sets display button text of employee resource group.
        /// </summary>
        public string RegisterERGButtonDisplayText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether QnA is enabled or not.
        /// </summary>
        public bool IsQnAEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether create ERG is restricted to global team or not.
        /// </summary>
        public bool IsERGCreationRestrictedToGlobalTeam { get; set; }
    }
}
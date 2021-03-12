// <copyright file="TeamUserPairUpMappingEntity.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Repositories.UserPairupMapping
{
    using Microsoft.Azure.Cosmos.Table;

    /// <summary>
    /// User pair up mapping entity class.
    /// This entity holds the information about a user pair up mapping.
    /// </summary>
    public class TeamUserPairUpMappingEntity : TableEntity
    {
        /// <summary>
        /// Gets or sets Azure Active Directory object id of user.
        /// </summary>
        public string UserObjectId
        {
            get { return this.PartitionKey; }
            set { this.PartitionKey = value; }
        }

        /// <summary>
        /// Gets or sets unique identifier for each team.
        /// </summary>
        public string TeamId
        {
            get { return this.RowKey; }
            set { this.RowKey = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether paused pair up matching is enabled or not for a given user.
        /// </summary>
        public bool IsPaused { get; set; }
    }
}
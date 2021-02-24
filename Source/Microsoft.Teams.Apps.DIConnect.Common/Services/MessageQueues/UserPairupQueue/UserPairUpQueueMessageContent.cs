// <copyright file="UserPairUpQueueMessageContent.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Services.MessageQueues.UserPairupQueue
{
    /// <summary>
    /// Azure service bus data queue message content class.
    /// </summary>
    public class UserPairUpQueueMessageContent
    {
        /// <summary>
        /// Gets or sets the pair-up notification id value.
        /// </summary>
        public string PairUpNotificationId { get; set; }

        /// <summary>
        /// Gets or sets the Team unique GUID id.
        /// </summary>
        public string TeamId { get; set; }

        /// <summary>
        /// Gets or sets the Team name.
        /// </summary>
        public string TeamName { get; set; }

        /// <summary>
        /// Gets or sets the pair up users data.
        /// </summary>
        public UserPairsMessage PairUpUserData { get; set; }
    }
}
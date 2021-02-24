// <copyright file="UserPairsMessage.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Services.MessageQueues.UserPairupQueue
{
    /// <summary>
    /// Azure service bus data queue message content class.
    /// </summary>
    public class UserPairsMessage
    {
        /// <summary>
        /// Gets or sets the recipient1 information.
        /// </summary>
        public UserData Recipient1 { get; set; }

        /// <summary>
        /// Gets or sets the recipient2 information.
        /// </summary>
        public UserData Recipient2 { get; set; }
    }
}
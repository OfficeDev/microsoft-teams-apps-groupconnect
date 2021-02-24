// <copyright file="PrepareToSendQueueMessageContent.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Services.MessageQueues.PrepareToSendQueue
{
    /// <summary>
    /// Azure service bus prepare to send queue message content class.
    /// </summary>
    public class PrepareToSendQueueMessageContent
    {
        /// <summary>
        /// Gets or sets the notification id value.
        /// </summary>
        public string NotificationId { get; set; }
    }
}
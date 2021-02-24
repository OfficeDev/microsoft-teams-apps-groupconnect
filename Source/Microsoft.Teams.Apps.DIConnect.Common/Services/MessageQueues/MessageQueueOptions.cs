// <copyright file="MessageQueueOptions.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Services.MessageQueues
{
    /// <summary>
    /// Options used for creating service bus message queues.
    /// </summary>
    public class MessageQueueOptions
    {
        /// <summary>
        /// Gets or sets the service bus connection.
        /// </summary>
        public string ServiceBusConnection { get; set; }
    }
}
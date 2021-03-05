// <copyright file="SendQueue.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>
namespace Microsoft.Teams.Apps.DIConnect.Common.Services.MessageQueues.SendQueue
{
    using Microsoft.Extensions.Options;

    /// <summary>
    /// The message queue service connected to the "di-connect-send" queue in Azure service bus.
    /// </summary>
    public class SendQueue : BaseQueue<SendQueueMessageContent>
    {
        /// <summary>
        /// Queue name of the send queue.
        /// </summary>
        public const string QueueName = "di-connect-send";

        /// <summary>
        /// Initializes a new instance of the <see cref="SendQueue"/> class.
        /// </summary>
        /// <param name="messageQueueOptions">The message queue options.</param>
        public SendQueue(IOptions<MessageQueueOptions> messageQueueOptions)
            : base(
                  serviceBusConnectionString: messageQueueOptions.Value.ServiceBusConnection,
                  queueName: SendQueue.QueueName)
        {
        }
    }
}
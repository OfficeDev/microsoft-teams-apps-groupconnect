// <copyright file="DataQueue.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Services.MessageQueues.DataQueue
{
    using Microsoft.Extensions.Options;

    /// <summary>
    /// The message queue service connected to the "di-connect-data" queue in Azure service bus.
    /// </summary>
    public class DataQueue : BaseQueue<DataQueueMessageContent>
    {
        /// <summary>
        /// Queue name of the data queue.
        /// </summary>
        public const string QueueName = "di-connect-data";

        /// <summary>
        /// Initializes a new instance of the <see cref="DataQueue"/> class.
        /// </summary>
        /// <param name="messageQueueOptions">The message queue options.</param>
        public DataQueue(IOptions<MessageQueueOptions> messageQueueOptions)
            : base(
                  serviceBusConnectionString: messageQueueOptions.Value.ServiceBusConnection,
                  queueName: DataQueue.QueueName)
        {
        }
    }
}
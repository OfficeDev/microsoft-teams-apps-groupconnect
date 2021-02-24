// <copyright file="ExportQueue.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Services.MessageQueues.ExportQueue
{
    using Microsoft.Extensions.Options;

    /// <summary>
    /// The message queue service connected to the "di-connect-export" queue in Azure service bus.
    /// </summary>
    public class ExportQueue : BaseQueue<ExportQueueMessageContent>
    {
        /// <summary>
        /// Queue name of the export queue.
        /// </summary>
        public const string QueueName = "di-connect-export";

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportQueue"/> class.
        /// </summary>
        /// <param name="messageQueueOptions">The message queue options.</param>
        public ExportQueue(IOptions<MessageQueueOptions> messageQueueOptions)
            : base(
                  serviceBusConnectionString: messageQueueOptions.Value.ServiceBusConnection,
                  queueName: ExportQueue.QueueName)
        {
        }
    }
}
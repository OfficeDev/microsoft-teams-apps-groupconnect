// <copyright file="UserPairUpQueue.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Services.MessageQueues.UserPairupQueue
{
    using Microsoft.Extensions.Options;

    /// <summary>
    /// The message queue service connected to the "user-pairup-data" queue in Azure service bus.
    /// </summary>
    public class UserPairUpQueue : BaseQueue<UserPairUpQueueMessageContent>
    {
        /// <summary>
        /// Queue name of the data queue.
        /// </summary>
        public const string QueueName = "user-pairup-data";

        /// <summary>
        /// Initializes a new instance of the <see cref="UserPairUpQueue"/> class.
        /// </summary>
        /// <param name="messageQueueOptions">The message queue options.</param>
        public UserPairUpQueue(IOptions<MessageQueueOptions> messageQueueOptions)
            : base(
                  serviceBusConnectionString: messageQueueOptions.Value.ServiceBusConnection,
                  queueName: UserPairUpQueue.QueueName)
        {
        }
    }
}
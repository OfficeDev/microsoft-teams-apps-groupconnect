// <copyright file="IPrepareToSendQueue.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Services.MessageQueues.PrepareToSendQueue
{
    /// <summary>
    /// interface for Prepare to send Queue.
    /// </summary>
    public interface IPrepareToSendQueue : IBaseQueue<PrepareToSendQueueMessageContent>
    {
    }
}
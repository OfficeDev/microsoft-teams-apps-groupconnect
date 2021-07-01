// <copyright file="ISendQueue.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Services.MessageQueues.SendQueue
{
    /// <summary>
    /// interface for Send Queue.
    /// </summary>
    public interface ISendQueue : IBaseQueue<SendQueueMessageContent>
    {
    }
}
// <copyright file="IUserPairUpQueue.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Services.MessageQueues.UserPairupQueue
{
    /// <summary>
    /// Interface for user pair up queue.
    /// </summary>
    public interface IUserPairUpQueue : IBaseQueue<UserPairUpQueueMessageContent>
    {
    }
}
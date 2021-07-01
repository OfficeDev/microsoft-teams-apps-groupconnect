// <copyright file="IDataQueue.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Services.MessageQueues.DataQueue
{
    /// <summary>
    /// interface for DataQueue.
    /// </summary>
    public interface IDataQueue : IBaseQueue<DataQueueMessageContent>
    {
    }
}
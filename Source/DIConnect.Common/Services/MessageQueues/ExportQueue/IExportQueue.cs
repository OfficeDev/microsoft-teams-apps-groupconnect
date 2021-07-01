// <copyright file="IExportQueue.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Services.MessageQueues.ExportQueue
{
    /// <summary>
    /// interface for Export Queue.
    /// </summary>
    public interface IExportQueue : IBaseQueue<ExportQueueMessageContent>
    {
    }
}
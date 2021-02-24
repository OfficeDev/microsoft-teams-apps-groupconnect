// <copyright file="DataQueueMessageOptions.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Prep.Func.PreparingToSend
{
    /// <summary>
    /// Options for data queue messages.
    /// </summary>
    public class DataQueueMessageOptions
    {
        /// <summary>
        /// Gets or sets the value for the delay to be applied to the data queue message.
        /// </summary>
        public double MessageDelayInSeconds { get; set; }
    }
}
// <copyright file="DataQueueMessageOptions.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Controllers
{
    /// <summary>
    /// Options for data queue messages.
    /// </summary>
    public class DataQueueMessageOptions
    {
        /// <summary>
        /// Gets or sets the value for the delay to be applied to the data queue message
        /// used to force mark a notification as complete if it is not already
        /// complete to ensure it is not left in a "sending" state.
        /// </summary>
        public double ForceCompleteMessageDelayInSeconds { get; set; }
    }
}
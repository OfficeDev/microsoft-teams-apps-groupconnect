// <copyright file="SendMessageResult.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Services.Teams
{
    /// <summary>
    /// Send message result enum.
    /// </summary>
    public enum SendMessageResult
    {
        /// <summary>
        /// Type indicating sending the notification succeeded.
        /// </summary>
        Succeeded,

        /// <summary>
        /// Type indicating sending the notification was throttled.
        /// </summary>
        Throttled,

        /// <summary>
        /// Type indicating sending the notification failed.
        /// </summary>
        Failed,

        /// <summary>
        /// Type indicating that the recipient can't be found.
        /// When sending a notification to a removed recipient, the send function gets 404 error.
        /// The recipient should be excluded from the list.
        /// </summary>
        RecipientNotFound,
    }
}
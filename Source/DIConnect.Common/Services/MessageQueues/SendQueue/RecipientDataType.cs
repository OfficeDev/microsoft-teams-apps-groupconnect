// <copyright file="RecipientDataType.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Services.MessageQueues.SendQueue
{
    /// <summary>
    /// Type used for indicating to the sending Azure Function what type of recipient to which
    /// the notification is to be delivered.
    /// </summary>
    public enum RecipientDataType
    {
        /// <summary>
        /// Indicates the notification is to be sent to a user.
        /// </summary>
        User,

        /// <summary>
        /// Indicates the notification is to be sent to a team.
        /// </summary>
        Team,
    }
}
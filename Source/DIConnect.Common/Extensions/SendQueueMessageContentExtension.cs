// <copyright file="SendQueueMessageContentExtension.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Extensions
{
    using System;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.MessageQueues.SendQueue;

    /// <summary>
    /// Extension class for <see cref="SendQueueMessageContent"/>.
    /// </summary>
    public static class SendQueueMessageContentExtension
    {
        /// <summary>
        /// Get service URL.
        /// </summary>
        /// <param name="message">Send Queue message.</param>
        /// <returns>Service URL.</returns>
        public static string GetServiceUrl(this SendQueueMessageContent message)
        {
            var recipient = message.RecipientData;
            return recipient.RecipientType switch
            {
                RecipientDataType.User => recipient.UserData.ServiceUrl,
                RecipientDataType.Team => recipient.TeamData.ServiceUrl,
                _ => throw new ArgumentException("Invalid recipient type"),
            };
        }

        /// <summary>
        /// Get conversationId.
        /// </summary>
        /// <param name="message">Send Queue message.</param>
        /// <returns>Conversation Id.</returns>
        public static string GetConversationId(this SendQueueMessageContent message)
        {
            var recipient = message.RecipientData;
            return recipient.RecipientType switch
            {
                RecipientDataType.User => recipient.UserData.ConversationId,
                RecipientDataType.Team => recipient.TeamData.TeamId,
                _ => throw new ArgumentException("Invalid recipient type"),
            };
        }
    }
}
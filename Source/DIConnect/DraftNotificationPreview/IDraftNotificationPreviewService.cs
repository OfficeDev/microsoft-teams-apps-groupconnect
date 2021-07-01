// <copyright file="IDraftNotificationPreviewService.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.DraftNotificationPreview
{
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.NotificationData;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.TeamData;

    /// <summary>
    /// Interface for DraftNotificationPreviewService.
    /// </summary>
    public interface IDraftNotificationPreviewService
    {
        /// <summary>
        /// Send a preview of a draft notification.
        /// </summary>
        /// <param name="draftNotificationEntity">Draft notification entity.</param>
        /// <param name="teamDataEntity">The team data entity.</param>
        /// <param name="teamsChannelId">The Teams channel id.</param>
        /// <returns>It returns HttpStatusCode.OK, if this method triggers the bot service to send the adaptive card successfully.
        /// It returns HttpStatusCode.TooManyRequests, if the bot service throttled the request to send the adaptive card.</returns>
        public Task<HttpStatusCode> SendPreview(NotificationDataEntity draftNotificationEntity, TeamDataEntity teamDataEntity, string teamsChannelId);
    }
}
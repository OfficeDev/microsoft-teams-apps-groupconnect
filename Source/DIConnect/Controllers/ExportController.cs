// <copyright file="ExportController.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Controllers
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Teams.Apps.DIConnect.Authentication;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.ExportData;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.SentNotificationData;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.UserData;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.MessageQueues.ExportQueue;

    /// <summary>
    /// Controller for exporting notification.
    /// </summary>
    [Route("api/exportnotification")]
    [Authorize(PolicyNames.MustBeAdminTeamMemberPolicy)]
    public class ExportController : Controller
    {
        private readonly SentNotificationDataRepository sentNotificationDataRepository;
        private readonly ExportDataRepository exportDataRepository;
        private readonly UserDataRepository userDataRepository;
        private readonly ExportQueue exportQueue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportController"/> class.
        /// </summary>
        /// <param name="sentNotificationDataRepository">SentNotification data repository instance.</param>
        /// <param name="exportDataRepository">Export data repository instance.</param>
        /// <param name="userDataRepository">User data repository instance.</param>
        /// <param name="exportQueue">The service bus queue for the export queue.</param>
        public ExportController(
            SentNotificationDataRepository sentNotificationDataRepository,
            ExportDataRepository exportDataRepository,
            UserDataRepository userDataRepository,
            ExportQueue exportQueue)
        {
            this.sentNotificationDataRepository = sentNotificationDataRepository;
            this.exportDataRepository = exportDataRepository;
            this.userDataRepository = userDataRepository;
            this.exportQueue = exportQueue;
        }

        /// <summary>
        /// Initiate a export of notification.
        /// </summary>
        /// <param name="id">notification id.</param>
        /// <returns>The result of an action method.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> ExportNotificationAsync(string id)
        {
            var userId = this.HttpContext.User.FindFirstValue(Common.Constants.ClaimTypeUserId);
            var user = await this.userDataRepository.GetAsync(UserDataTableNames.UserDataPartition, userId);
            if (user == null)
            {
                return this.NotFound();
            }

            // Ensure the data tables needed by the Azure Function to export the notification exist in Azure storage.
            await Task.WhenAll(
                this.sentNotificationDataRepository.EnsureSentNotificationDataTableExistsAsync(),
                this.exportDataRepository.EnsureExportDataTableExistsAsync());
            var exportNotification = await this.exportDataRepository.GetAsync(userId, id);
            if (exportNotification != null)
            {
                return this.Conflict();
            }

            await this.exportDataRepository.CreateOrUpdateAsync(new ExportDataEntity()
            {
                PartitionKey = userId,
                RowKey = id,
                SentDate = DateTime.UtcNow,
                Status = ExportStatus.New.ToString(),
            });

            var exportQueueMessageContent = new ExportQueueMessageContent
            {
                NotificationId = id,
                UserId = userId,
            };
            await this.exportQueue.SendAsync(exportQueueMessageContent);

            return this.Ok();
        }
    }
}
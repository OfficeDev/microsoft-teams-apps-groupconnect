// <copyright file="ExportFunction.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Prep.Func
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using Microsoft.Extensions.Localization;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.ExportData;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.NotificationData;
    using Microsoft.Teams.Apps.DIConnect.Common.Resources;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.MessageQueues.ExportQueue;
    using Microsoft.Teams.Apps.DIConnect.Prep.Func.Export.Model;
    using Microsoft.Teams.Apps.DIConnect.Prep.Func.Export.Orchestrator;
    using Newtonsoft.Json;

    /// <summary>
    /// Azure Function App triggered by messages from a Service Bus queue.
    /// This function exports notification in a zip file for the admin.
    /// It prepares the file by reading the notification data, user graph api.
    /// This function stage the file in Blob Storage and send the
    /// file card to the admin using bot framework adapter.
    /// </summary>
    public class ExportFunction
    {
        private readonly NotificationDataRepository notificationDataRepository;
        private readonly ExportDataRepository exportDataRepository;
        private readonly IStringLocalizer<Strings> localizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportFunction"/> class.
        /// </summary>
        /// <param name="notificationDataRepository">Notification data repository.</param>
        /// <param name="exportDataRepository">Export data repository.</param>
        /// <param name="localizer">Localization service.</param>
        public ExportFunction(
            NotificationDataRepository notificationDataRepository,
            ExportDataRepository exportDataRepository,
            IStringLocalizer<Strings> localizer)
        {
            this.notificationDataRepository = notificationDataRepository;
            this.exportDataRepository = exportDataRepository;
            this.localizer = localizer;
        }

        /// <summary>
        /// Azure Function App triggered by messages from a Service Bus queue.
        /// It kicks off the durable orchestration for exporting notifications.
        /// </summary>
        /// <param name="myQueueItem">The Service Bus queue item.</param>
        /// <param name="starter">Durable orchestration client.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [FunctionName("DIConnectExportFunction")]
        public async Task Run(
            [ServiceBusTrigger(
             ExportQueue.QueueName,
             Connection = ExportQueue.ServiceBusConnectionConfigurationKey)]
            string myQueueItem,
            [DurableClient]
            IDurableOrchestrationClient starter)
        {
            var messageContent = JsonConvert.DeserializeObject<ExportMessageQueueContent>(myQueueItem);
            var notificationId = messageContent.NotificationId;

            var sentNotificationDataEntity = await this.notificationDataRepository.GetAsync(
                partitionKey: NotificationDataTableNames.SentNotificationsPartition,
                rowKey: notificationId);
            var exportDataEntity = await this.exportDataRepository.GetAsync(messageContent.UserId, notificationId);
            exportDataEntity.FileName = this.GetFileName();
            var requirement = new ExportDataRequirement(sentNotificationDataEntity, exportDataEntity, messageContent.UserId);
            if (requirement.IsValid())
            {
                string instanceId = await starter.StartNewAsync(
                    nameof(ExportOrchestration.ExportOrchestrationAsync),
                    requirement);
            }
        }

        private string GetFileName()
        {
            var guid = Guid.NewGuid().ToString();
            var fileName = this.localizer.GetString("FileName_ExportData");
            return $"{fileName}_{guid}.zip";
        }
    }
}
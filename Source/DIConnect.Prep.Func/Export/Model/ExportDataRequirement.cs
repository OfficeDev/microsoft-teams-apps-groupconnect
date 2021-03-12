// <copyright file="ExportDataRequirement.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Prep.Func.Export.Model
{
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.ExportData;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.NotificationData;

    /// <summary>
    /// Export data requirement model class.
    /// </summary>
    public class ExportDataRequirement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportDataRequirement"/> class.
        /// </summary>
        /// <param name="notificationDataEntity">the notification data entity.</param>
        /// <param name="exportDataEntity">the export data entity.</param>
        /// <param name="userId">user id.</param>
        public ExportDataRequirement(
            NotificationDataEntity notificationDataEntity,
            ExportDataEntity exportDataEntity,
            string userId)
        {
            this.NotificationDataEntity = notificationDataEntity;
            this.ExportDataEntity = exportDataEntity;
            this.UserId = userId;
        }

        /// <summary>
        /// Gets the user id.
        /// </summary>
        public string UserId { get; private set; }

        /// <summary>
        /// Gets the notification data entity.
        /// </summary>
        public NotificationDataEntity NotificationDataEntity { get; private set; }

        /// <summary>
        /// Gets the export data entity.
        /// </summary>
        public ExportDataEntity ExportDataEntity { get; private set; }

        /// <summary>
        /// Check if requirement is met.
        /// </summary>
        /// <returns>value to determine if requirement is met.</returns>
        public bool IsValid()
        {
            return this.NotificationDataEntity != null && this.ExportDataEntity != null;
        }
    }
}
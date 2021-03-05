// <copyright file="ExportStatus.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Repositories.ExportData
{
    /// <summary>
    /// Export telemetry status.
    /// </summary>
    public enum ExportStatus
    {
        /// <summary>
        /// This represents the export is scheduled.
        /// </summary>
        New,

        /// <summary>
        /// This represents the export is in progress.
        /// </summary>
        InProgress,

        /// <summary>
        /// This represents the export is completed.
        /// </summary>
        Completed,

        /// <summary>
        /// This represents the export is failed.
        /// </summary>
        Failed,
    }
}
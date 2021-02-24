// <copyright file="ExportDataTableName.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Repositories.ExportData
{
    /// <summary>
    /// Export data table names.
    /// </summary>
    public class ExportDataTableName
    {
        /// <summary>
        /// Table name for the send batches data table.
        /// </summary>
        public static readonly string TableName = "ExportData";

        /// <summary>
        /// Default partition - should not be used.
        /// </summary>
        public static readonly string DefaultPartition = "Default";
    }
}
// <copyright file="AppConfigTableName.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Repositories
{
    /// <summary>
    /// App config table information.
    /// </summary>
    public class AppConfigTableName
    {
        /// <summary>
        /// Table name for app config..
        /// </summary>
        public static readonly string TableName = "AppConfig";

        /// <summary>
        /// App settings partition.
        /// </summary>
        public static readonly string SettingsPartition = "Settings";

        /// <summary>
        /// Service URL row key.
        /// </summary>
        public static readonly string ServiceUrlRowKey = "ServiceUrl";

        /// <summary>
        /// User app id row key.
        /// </summary>
        public static readonly string UserAppIdRowKey = "UserAppId";

        /// <summary>
        /// Knowledge base id row key.
        /// </summary>
        public static readonly string KnowledgeBaseIdRowKey = "KnowledgeBaseId";
    }
}
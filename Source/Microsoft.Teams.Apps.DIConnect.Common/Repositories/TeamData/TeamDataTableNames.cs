// <copyright file="TeamDataTableNames.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Repositories.TeamData
{
    /// <summary>
    /// Team data table names.
    /// </summary>
    public static class TeamDataTableNames
    {
        /// <summary>
        /// Table name for the team data table.
        /// </summary>
        public static readonly string TableName = "TeamData";

        /// <summary>
        /// Team data partition key name.
        /// </summary>
        public static readonly string TeamDataPartition = "TeamData";
    }
}
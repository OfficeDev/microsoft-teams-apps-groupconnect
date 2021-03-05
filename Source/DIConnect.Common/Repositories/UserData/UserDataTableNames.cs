// <copyright file="UserDataTableNames.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Repositories.UserData
{
    /// <summary>
    /// User data table names.
    /// </summary>
    public static class UserDataTableNames
    {
        /// <summary>
        /// Table name for the user data table.
        /// </summary>
        public static readonly string TableName = "UserData";

        /// <summary>
        /// Users data partition key name.
        /// </summary>
        public static readonly string UserDataPartition = "UserData";

        /// <summary>
        /// Users sync data partition.
        /// </summary>
        public static readonly string UsersSyncDataPartition = "UsersSyncData";

        /// <summary>
        /// All users delta link row key.
        /// </summary>
        public static readonly string AllUsersDeltaLinkRowKey = "AllUsersDeltaLink";
    }
}
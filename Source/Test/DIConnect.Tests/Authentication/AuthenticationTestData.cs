// <copyright file="AuthenticationTestData.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Tests.Authentication
{
    using Microsoft.Graph;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.UserData;
    using System.Collections.Generic;

    /// <summary>
    /// Class that contains test methods for authentication helper.
    /// </summary>
    public class AuthenticationTestData
    {
        /// <summary>
        /// Represents unique list of team owners id's.
        /// </summary>
        public static ISet<string> teamOwnersList = new HashSet<string>() { "1a1cce71-2833-4345-86e2-e9047f73e6af", "8794854-8794-4345-86e2-e9047f73e6af" };

        /// <summary>
        /// Represents user object id.
        /// </summary>
        public static string userObjectId = "1a1cce71-2833-4345-86e2-e9047f73e6af";

        /// <summary>
        /// Represents list of users.
        /// </summary>
        public static IEnumerable<User> users = new List<User>()
        {
            new User()
            {
                Id = "1a1cce71-2833-4345-86e2-e9047f73e6af"
            },
            new User()
            {
                Id = "894123-1548-4345-86e2-e9047f73e6af"
            },
        };

        /// <summary>
        /// Represents list of user data.
        /// </summary>
        public static IEnumerable<UserDataEntity> userDataEntities = new List<UserDataEntity>()
        {
            new UserDataEntity()
            {
                AadId = "123"
            },
            new UserDataEntity()
            {
                AadId ="456"
            },
        };
    }
}
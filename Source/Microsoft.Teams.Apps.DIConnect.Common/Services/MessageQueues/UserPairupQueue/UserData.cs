// <copyright file="UserData.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Services.MessageQueues.UserPairupQueue
{
    /// <summary>
    /// User data object for message content class.
    /// </summary>
    public class UserData
    {
        /// <summary>
        /// Gets or sets the user principal name.
        /// </summary>
        public string UserPrincipalName { get; set; }

        /// <summary>
        /// Gets or sets the user given name.
        /// </summary>
        public string UserGivenName { get; set; }

        /// <summary>
        /// Gets or sets Azure Active Directory of user.
        /// </summary>
        public string UserObjectId { get; set; }
    }
}
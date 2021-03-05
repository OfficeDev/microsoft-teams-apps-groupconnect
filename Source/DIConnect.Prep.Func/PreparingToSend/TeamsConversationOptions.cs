// <copyright file="TeamsConversationOptions.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Prep.Func
{
    /// <summary>
    /// Options for Teams Conversation.
    /// </summary>
    public class TeamsConversationOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether user app should be pro-actively installed.
        /// </summary>
        public bool ProactivelyInstallUserApp { get; set; }

        /// <summary>
        /// Gets or sets maximum attempts to create conversation with teams user.
        /// </summary>
        public int MaxAttemptsToCreateConversation { get; set; }
    }
}
// <copyright file="UserAppCredentials.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Services.CommonBot
{
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// A user Microsoft app credentials object.
    /// </summary>
    public class UserAppCredentials : MicrosoftAppCredentials
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserAppCredentials"/> class.
        /// </summary>
        /// <param name="botOptions">The bot options.</param>
        public UserAppCredentials(IOptions<BotOptions> botOptions)
            : base(
                  appId: botOptions.Value.UserAppId,
                  password: botOptions.Value.UserAppPassword)
        {
        }
    }
}

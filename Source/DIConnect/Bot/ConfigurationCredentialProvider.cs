// <copyright file="ConfigurationCredentialProvider.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Bot
{
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.CommonBot;

    /// <summary>
    /// This class implements ICredentialProvider, which is used by the bot framework to retrieve credential info.
    /// </summary>
    public class ConfigurationCredentialProvider : SimpleCredentialProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationCredentialProvider"/> class.
        /// </summary>
        /// <param name="botOptions">The bot options.</param>
        public ConfigurationCredentialProvider(IOptions<BotOptions> botOptions)
            : base(
                appId: botOptions.Value.MicrosoftAppId,
                password: botOptions.Value.MicrosoftAppPassword)
        {
        }
    }
}
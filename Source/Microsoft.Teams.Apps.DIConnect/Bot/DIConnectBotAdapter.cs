// <copyright file="DIConnectBotAdapter.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Bot
{
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Bot.Connector.Authentication;

    /// <summary>
    /// The DI Connect Bot Adapter.
    /// </summary>
    public class DIConnectBotAdapter : BotFrameworkHttpAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DIConnectBotAdapter"/> class.
        /// </summary>
        /// <param name="credentialProvider">Credential provider service instance.</param>
        /// <param name="dIConnectBotFilterMiddleware">Teams message filter middleware instance.</param>
        public DIConnectBotAdapter(
            ICredentialProvider credentialProvider,
            DIConnectBotFilterMiddleware dIConnectBotFilterMiddleware)
            : base(credentialProvider)
        {
            this.Use(dIConnectBotFilterMiddleware);
        }
    }
}
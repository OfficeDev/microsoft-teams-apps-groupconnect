// <copyright file="DIBotFrameworkHttpAdapter.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Adapter
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Bot.Schema;

    /// <summary>
    /// Bot framework http adapter instance.
    /// </summary>
    public class DIBotFrameworkHttpAdapter : BotFrameworkHttpAdapter, IDIBotFrameworkHttpAdapter
    {
        private readonly ICredentialProvider credentialProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DIBotFrameworkHttpAdapter"/> class.
        /// </summary>
        /// <param name="credentialProvider">credential provider.</param>
        public DIBotFrameworkHttpAdapter(ICredentialProvider credentialProvider)
            : base(credentialProvider)
        {
            this.credentialProvider = credentialProvider;
        }

        /// <inheritdoc/>
        public override Task CreateConversationAsync(string channelId, string serviceUrl, MicrosoftAppCredentials credentials, ConversationParameters conversationParameters, BotCallbackHandler callback, CancellationToken cancellationToken)
        {
            return base.CreateConversationAsync(channelId, serviceUrl, credentials, conversationParameters, callback, cancellationToken);
        }
    }
}
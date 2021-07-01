// <copyright file="ChatsService.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Services.MicrosoftGraph
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Graph;

    /// <summary>
    /// Chats Service.
    /// </summary>
    internal class ChatsService : IChatsService
    {
        private readonly IAppManagerService appManagerService;
        private readonly IGraphServiceClient graphServiceClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatsService"/> class.
        /// </summary>
        /// <param name="graphServiceClient">Graph service client.</param>
        /// <param name="appManagerService">App manager service.</param>
        internal ChatsService(
            IGraphServiceClient graphServiceClient,
            IAppManagerService appManagerService)
        {
            this.graphServiceClient = graphServiceClient ?? throw new ArgumentNullException(nameof(graphServiceClient));
            this.appManagerService = appManagerService ?? throw new ArgumentNullException(nameof(appManagerService));
        }

        /// <inheritdoc/>
        public async Task<string> GetChatThreadIdAsync(string userId, string appId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            if (string.IsNullOrWhiteSpace(appId))
            {
                throw new ArgumentNullException(nameof(appId));
            }

            var installationId = await this.appManagerService.GetAppInstallationIdForUserAsync(appId, userId);
            var chat = await this.graphServiceClient.Users[userId]
                .Teamwork
                .InstalledApps[installationId]
                .Chat
                .Request()
                .WithMaxRetry(GraphConstants.MaxRetry)
                .GetAsync();

            return chat?.Id;
        }
    }
}
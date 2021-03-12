// <copyright file="BotController.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Teams.Apps.DIConnect.Bot;

    /// <summary>
    /// Message controller for the bot.
    /// </summary>
    [Route("api/messages")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly BotFrameworkHttpAdapter adapter;
        private readonly IBot bot;

        /// <summary>
        /// Initializes a new instance of the <see cref="BotController"/> class.
        /// Dependency Injection will provide the Adapter and IBot implementation at runtime.
        /// </summary>
        /// <param name="adapter">DI Connect Bot Adapter instance.</param>
        /// <param name="bot">DI Connect Bot instance.</param>
        public BotController(DIConnectBotAdapter adapter, IBot bot)
        {
            this.adapter = adapter;
            this.bot = bot;
        }

        /// <summary>
        /// POST: api/Messages
        /// Delegate the processing of the HTTP POST to the adapter.
        /// The adapter will invoke the bot.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [HttpPost]
        public async Task PostAsync()
        {
            await this.adapter.ProcessAsync(this.Request, this.Response, this.bot);
        }
    }
}
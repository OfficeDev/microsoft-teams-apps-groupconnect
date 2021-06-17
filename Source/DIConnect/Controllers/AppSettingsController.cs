// <copyright file="AppSettingsController.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Controllers
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.CommonBot;
    using Microsoft.Teams.Apps.DIConnect.Models;

    /// <summary>
    /// Controller to get app settings.
    /// </summary>
    [Route("api/settings")]
    [ApiController]
    public class AppSettingsController : ControllerBase
    {
        private readonly BotOptions botOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettingsController"/> class.
        /// </summary>
        /// <param name="userAppOptions">User app options.</param>
        public AppSettingsController(
            IOptions<BotOptions> userAppOptions)
        {
            this.botOptions = userAppOptions?.Value ?? throw new ArgumentNullException(nameof(userAppOptions));
        }

        /// <summary>
        /// Get app id.
        /// </summary>
        /// <returns>Required sent notification.</returns>
        [HttpGet]
        public IActionResult GetUserAppId()
        {
            var appId = this.botOptions.MicrosoftAppId;
            var onlyAdminsRegisterERG = this.botOptions.OnlyAdminsRegisterERG;
            var response = new AppConfigurations()
            {
                AppId = appId,
                OnlyAdminsRegisterERG = onlyAdminsRegisterERG,
            };

            return this.Ok(response);
        }
    }
}
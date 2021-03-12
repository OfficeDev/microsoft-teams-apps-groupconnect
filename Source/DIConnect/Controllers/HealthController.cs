// <copyright file="HealthController.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Controller for health endpoint.
    /// </summary>
    [Route("[controller]")]
    public class HealthController : Controller
    {
        /// <summary>
        /// Report health status of the application.
        /// </summary>
        /// <returns>Action.</returns>
        [HttpGet]
        public ActionResult Index()
        {
            return this.Ok();
        }
    }
}
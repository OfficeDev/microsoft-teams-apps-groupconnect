// <copyright file="AppConfigurations.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Models
{
    /// <summary>
    /// Application configuration data model class.
    /// </summary>
    public class AppConfigurations
    {
        /// <summary>
        /// Gets or sets the Microsoft app ID for the bot.
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// Gets or sets application OnlyAdminsRegisterERG.
        /// </summary>
        public string OnlyAdminsRegisterERG { get; set; }
    }
}
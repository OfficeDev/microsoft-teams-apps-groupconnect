// <copyright file="LocaleOptions.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Resources
{
    /// <summary>
    /// Options used for setting locale.
    /// </summary>
    public class LocaleOptions
    {
        /// <summary>
        /// Gets or sets the default culture.
        /// </summary>
        public string DefaultCulture { get; set; }

        /// <summary>
        /// Gets or sets the supported cultures.
        /// </summary>
        public string SupportedCultures { get; set; }
    }
}
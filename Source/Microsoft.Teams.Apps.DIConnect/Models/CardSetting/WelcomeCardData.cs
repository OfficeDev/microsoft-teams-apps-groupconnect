// <copyright file="WelcomeCardData.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Models.CardSetting
{
    using Newtonsoft.Json;

    /// <summary>
    /// Welcome card data class.
    /// This entity holds the content required for welcome card.
    /// </summary>
    public class WelcomeCardData
    {
        /// <summary>
        /// Gets or sets welcome title text value.
        /// </summary>
        [JsonProperty("welcomeTitleText")]
        public string WelcomeTitleText { get; set; }

        /// <summary>
        /// Gets or sets welcome header text value.
        /// </summary>
        [JsonProperty("welcomeHeaderText")]
        public string WelcomeHeaderText { get; set; }

        /// <summary>
        /// Gets or sets discover groups bullet text value.
        /// </summary>
        [JsonProperty("discoverGroupsBulletText")]
        public string DiscoverGroupsBulletText { get; set; }

        /// <summary>
        /// Gets or sets meet people bullet text value.
        /// </summary>
        [JsonProperty("meetPeopleBulletText")]
        public string MeetPeopleBulletText { get; set; }

        /// <summary>
        /// Gets or sets answer bullet text value.
        /// </summary>
        [JsonProperty("getAnswersBulletText")]
        public string GetAnswersBulletText { get; set; }

        /// <summary>
        /// Gets or sets about groups bullet text value.
        /// </summary>
        [JsonProperty("aboutGroupsBulletText")]
        public string AboutGroupsBulletText { get; set; }

        /// <summary>
        /// Gets or sets discover groups button text value.
        /// </summary>
        [JsonProperty("discoverGroupsButtonText")]
        public string DiscoverGroupsButtonText { get; set; }

        /// <summary>
        /// Gets or sets discover groups url value.
        /// </summary>
        [JsonProperty("discoverGroupsUrl")]
        public string DiscoverGroupsUrl { get; set; }
    }
}
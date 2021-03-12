// <copyright file="QnAMakerSettings.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Services
{
    /// <summary>
    /// Provides app settings related to QnA maker service provider.
    /// </summary>
    public class QnAMakerSettings
    {
        /// <summary>
        /// Gets or Sets score threshold to get answer from knowledge base for a given question.
        /// </summary>
        public double ScoreThreshold { get; set; }
    }
}
// <copyright file="MatchingFrequency.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Repositories.EmployeeResourceGroup
{
    /// <summary>
    /// An enum that represent the pair-up matching frequency (Monthly/Weekly) for a given Team.
    /// </summary>
    public enum MatchingFrequency
    {
        /// <summary>
        ///  Matching frequency weekly.
        /// </summary>
        Weekly = 0,

        /// <summary>
        /// Matching frequency monthly.
        /// </summary>
        Monthly = 1,
    }
}
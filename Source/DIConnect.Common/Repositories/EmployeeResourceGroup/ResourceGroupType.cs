// <copyright file="ResourceGroupType.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Repositories.EmployeeResourceGroup
{
    /// <summary>
    /// A enum that represent the resource group type like External/Teams.
    /// </summary>
    public enum ResourceGroupType
    {
        /// <summary>
        /// Group type as Teams.
        /// </summary>
        Teams,

        /// <summary>
        /// Group type as External.
        /// </summary>
        External,
    }
}
// <copyright file="TableRowKeyGenerator.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Repositories
{
    using System;

    /// <summary>
    /// This class generates rowKeys based off timestamps so that the order of the table is correct.
    /// </summary>
    public class TableRowKeyGenerator
    {
        /// <summary>
        /// Generates a new row key based off of the current timestamp such that the keys are ordered most recent => oldest.
        /// </summary>
        /// <returns>A new row key.</returns>
        public string CreateNewKeyOrderingMostRecentToOldest()
        {
            var invertedTicksString = string.Format("{0:D19}", DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks);

            return invertedTicksString;
        }

        /// <summary>
        /// Generates a new row key based off of the current timestamp such that the keys are ordered oldest => most recent.
        /// </summary>
        /// <returns>A new row key.</returns>
        public string CreateNewKeyOrderingOldestToMostRecent()
        {
            var ticksString = string.Format("{0:D19}", DateTime.UtcNow.Ticks);

            return ticksString;
        }
    }
}
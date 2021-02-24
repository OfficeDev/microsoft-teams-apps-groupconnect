// <copyright file="MetaData.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Prep.Func.Export.Model
{
    using System;

    /// <summary>
    /// MetaData model class.
    /// </summary>
    public class Metadata
    {
        /// <summary>
        /// Gets or sets the message title.
        /// </summary>
        public string MessageTitle { get; set; }

        /// <summary>
        /// Gets or sets the sent timestamp.
        /// </summary>
        public DateTime? SentTimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the export timestamp.
        /// </summary>
        public DateTime? ExportTimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the exported by user id.
        /// </summary>
        public string ExportedBy { get; set; }
    }
}
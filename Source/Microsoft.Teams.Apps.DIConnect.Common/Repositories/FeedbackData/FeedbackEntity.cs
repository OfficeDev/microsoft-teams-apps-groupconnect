// <copyright file="FeedbackEntity.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.DIConnect.Common.FeedbackData
{
    using System;
    using Microsoft.Azure.Cosmos.Table;

    /// <summary>
    /// Class contains shared feedback details.
    /// </summary>
    public class FeedbackEntity : TableEntity
    {
        /// <summary>
        /// Gets or sets Feedback.
        /// </summary>
        public string Feedback { get; set; }

        /// <summary>
        /// Gets or sets user Azure Active Directory object id.
        /// </summary>
        public string UserAadObjectId
        {
            get
            {
                return this.PartitionKey;
            }

            set
            {
                this.PartitionKey = value;
            }
        }

        /// <summary>
        /// Gets or sets unique feedback id.
        /// </summary>
        public string FeedbackId
        {
            get
            {
                return this.RowKey;
            }

            set
            {
                this.RowKey = value;
            }
        }

        /// <summary>
        /// Gets or sets survey submission date and time.
        /// </summary>
        public DateTime SubmittedOn { get; set; }
    }
}
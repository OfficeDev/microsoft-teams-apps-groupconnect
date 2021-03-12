// <copyright file="SendFunctionOptions.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Send.Func
{
    /// <summary>
    /// Options used to configure the DI Connect Send Function.
    /// </summary>
    public class SendFunctionOptions
    {
        /// <summary>
        /// Gets or sets the max number of request attempts.
        /// </summary>
        public int MaxNumberOfAttempts { get; set; }

        /// <summary>
        /// Gets or sets the number of seconds to delay before
        /// retrying to send the message.
        /// </summary>
        public double SendRetryDelayNumberOfSeconds { get; set; }
    }
}
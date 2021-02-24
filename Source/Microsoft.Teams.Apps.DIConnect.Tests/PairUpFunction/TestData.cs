// <copyright file="TestData.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Tests.PairUpFunction
{
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.MessageQueues;
   
    /// <summary>
    /// Class for test data.
    /// </summary>
    public static class TestData
    {
        /// <summary>
        /// Method to generate test data for message queue options.
        /// </summary>
        public static readonly IOptions<MessageQueueOptions> messageQueueOptions = Options.Create(new MessageQueueOptions()
        {
            ServiceBusConnection = ""
        });

        /// <summary>
        /// Method to generate test data for repository options.
        /// </summary>
        public static readonly IOptions<RepositoryOptions> repositoryOptions = Options.Create(new RepositoryOptions()
        {
            StorageAccountConnectionString = "",
            EnsureTableExists = false
        });
    }
}
// <copyright file="SendPairUpNotificationFunctionTest.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Tests.PairUpFunction
{
    using Moq;
    using System;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.UserData;
    using Microsoft.Teams.Apps.DIConnect.Common.Resources;
    using Microsoft.Teams.Apps.DIConnect.Common.Services;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.Teams;
    using Microsoft.Teams.Apps.DIConnect.Send.Func;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Extensions.Caching.Memory;

    /// <summary>
    /// Class to test send pair up notification function.
    /// </summary>
    [TestClass]
    public class SendPairUpNotificationFunctionTest
    {
        Mock<UserDataRepository> userDataRepository;
        Mock<IMessageService> messageService;
        Mock<IAppSettingsService> appSettingsService;
        Mock<IStringLocalizer<Strings>> localizer;
        Mock<IMemoryCache> memoryCache;

        SendPairUpNotificationFunction sendPairUpNotificationFunction;

        /// <summary>
        /// Test initializer for send pair up notification function.
        /// </summary>
        [TestInitialize]
        public void SendPairUpNotificationFunctionTestSetup()
        {
            localizer = new Mock<IStringLocalizer<Strings>>();
            var userDataLogger = new Mock<ILogger<UserDataRepository>>().Object;
            userDataRepository = new Mock<UserDataRepository>(userDataLogger, TestData.repositoryOptions);
            messageService = new Mock<IMessageService>();
            appSettingsService = new Mock<IAppSettingsService>();
            memoryCache = new Mock<IMemoryCache>();

            sendPairUpNotificationFunction = new SendPairUpNotificationFunction(
                messageService.Object,
                userDataRepository.Object,
                appSettingsService.Object,
                memoryCache.Object,
                localizer.Object);
        }

        /// <summary>
        /// Test method to verify argument null exceptions for send pair up notification function.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SendPairUpNotificationFunction_ThrowsMessageServiceArgumentNullException()
        {
            new SendPairUpNotificationFunction(
                null,
                userDataRepository.Object,
                appSettingsService.Object,
                memoryCache.Object,
                localizer.Object);
        }

        /// <summary>
        /// Test method to verify argument null exceptions for user data.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SendPairUpNotificationFunction_ThrowsUserDataRepositoryArgumentNullException()
        {
            new SendPairUpNotificationFunction(
                messageService.Object,
                null,
                appSettingsService.Object,
                memoryCache.Object,
                localizer.Object);
        }

        /// <summary>
        /// Test method to verify argument null exceptions for setting service.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SendPairUpNotificationFunction_ThrowsSettingServiceArgumentNullException()
        {
            new SendPairUpNotificationFunction(
                messageService.Object,
                userDataRepository.Object,
                null,
                memoryCache.Object,
                localizer.Object);
        }

        /// <summary>
        /// Test method to verify argument null exceptions for localizer argument.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SendPairUpNotificationFunction_ThrowsLocalizerArgumentNullException()
        {
            new SendPairUpNotificationFunction(
                messageService.Object,
                userDataRepository.Object,
                appSettingsService.Object,
                memoryCache.Object,
                localizer.Object);
        }

        /// <summary>
        /// Test method to verify argument null exceptions for memory cache.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SendPairUpNotificationFunction_ThrowsMemoryCacheArgumentNullException()
        {
            new SendPairUpNotificationFunction(
                messageService.Object,
                userDataRepository.Object,
                appSettingsService.Object,
                null,
                localizer.Object);
        }
    }
}
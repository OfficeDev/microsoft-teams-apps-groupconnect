// <copyright file="MustBeTeamOwnerOrAdminUserHandlerTest.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Tests.Authentication.PolicyHandlerTest
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.DIConnect.Authentication;
    using Microsoft.Teams.Apps.DIConnect.Authentication.AuthenticationHelper;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.MicrosoftGraph;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// Test class for Team owner or admin user policy handler.
    /// </summary>
    [TestClass]
    public class MustBeTeamOwnerOrAdminUserHandlerTest
    {
        private Mock<IMemberValidationHelper> memberValidationHelper;
        private Mock<IGroupsService> groupService;
        private Mock<IMemoryCache> memoryCache;
        private Mock<ILogger<MustBeTeamOwnerOrAdminUserHandler>> logger;
        private MustBeTeamOwnerOrAdminUserHandler policyHandler;

        private AuthorizationHandlerContext authContext;

        /// <summary>
        /// Initialize all test variables.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.memberValidationHelper = new Mock<IMemberValidationHelper>();
            this.groupService = new Mock<IGroupsService>();
            this.memoryCache = new Mock<IMemoryCache>();
            this.logger = new Mock<ILogger<MustBeTeamOwnerOrAdminUserHandler>>();

            this.policyHandler = new MustBeTeamOwnerOrAdminUserHandler(
                this.memberValidationHelper.Object,
                this.groupService.Object,
                this.memoryCache.Object,
                this.logger.Object);
        }

        /// <summary>
        /// Validate auth handle for success.
        /// </summary>
        /// <returns><see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task ValidateHandleAsync_AdminTeamMemberSucceed()
        {
            // Arrange
            this.memberValidationHelper
                .Setup(svc => svc.IsAdminTeamMemberAsync(It.IsAny<string>()))
                .ReturnsAsync(() => true);

            this.authContext = FakeHttpContext.GetAuthorizationHandlerContextForTeamOwnerOrAdminUser();

            // Act
            await this.policyHandler.HandleAsync(this.authContext);

            // Assert
            Assert.IsTrue(this.authContext.HasSucceeded);
        }

        /// <summary>
        /// Validate auth handle for success.
        /// </summary>
        /// <returns><see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task ValidateHandleAsync_TeamOwnerSucceed()
        {
            // Arrange
            this.memoryCache
                .Setup(x => x.CreateEntry(It.IsAny<string>()))
                .Returns(Mock.Of<ICacheEntry>);

            this.groupService
                .Setup(svc => svc.GetTeamOwnersAadObjectIdAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(AuthenticationTestData.teamOwnersList));


            this.authContext = FakeHttpContext.GetAuthorizationHandlerContextForTeamOwner();

            // Act
            await this.policyHandler.HandleAsync(this.authContext);

            // Assert
            Assert.IsTrue(this.authContext.HasSucceeded);
        }

        /// <summary>
        /// Validate auth handle for failed.
        /// </summary>
        /// <returns><see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task ValidateHandleAsync_Failed()
        {
            // Arrange
            this.memberValidationHelper
                .Setup(svc => svc.IsAdminTeamMemberAsync(It.IsAny<string>()))
                .ReturnsAsync(() => false);

            this.authContext = FakeHttpContext.GetAuthorizationHandlerContextForTeamOwnerOrAdminUser();

            // Act
            await this.policyHandler.HandleAsync(this.authContext);

            // Assert
            Assert.IsFalse(this.authContext.HasSucceeded);
        }
    }
}
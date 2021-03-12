// <copyright file="MustBeTeamMemberHandlerTest.cs" company="Microsoft Corporation">
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
    using Microsoft.Teams.Apps.DIConnect.Common.Services.MicrosoftGraph;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// Test class for team member policy handler.
    /// </summary>
    [TestClass]
    public class MemberValidationHelperTest
    {
        private Mock<IGroupMembersService> groupMemberService;
        private Mock<IMemoryCache> memoryCache;
        private Mock<ILogger<MustBeTeamMemberHandler>> logger;

        private MustBeTeamMemberHandler policyHandler;
        private AuthorizationHandlerContext authContext;
        

        /// <summary>
        /// Initialize all test variables.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.groupMemberService = new Mock<IGroupMembersService>();
            this.memoryCache = new Mock<IMemoryCache>();
            this.logger = new Mock<ILogger<MustBeTeamMemberHandler>>();

            this.policyHandler = new MustBeTeamMemberHandler(
                this.groupMemberService.Object,
                this.memoryCache.Object,
                this.logger.Object);
        }

        /// <summary>
        /// Validate auth handle for success.
        /// </summary>
        /// <returns><see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task ValidateHandleAsync_Succeed()
        {
            // Arrange
            this.memoryCache
                .Setup(x => x.CreateEntry(It.IsAny<string>()))
                .Returns(Mock.Of<ICacheEntry>);

            this.groupMemberService
                .Setup(svc => svc.GetGroupMembersAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(AuthenticationTestData.users));

            this.authContext = FakeHttpContext.GetAuthorizationHandlerContextForTeamMember();

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
            this.memoryCache
                .Setup(x => x.CreateEntry(It.IsAny<string>()))
                .Returns(Mock.Of<ICacheEntry>);

            this.groupMemberService
                .Setup(svc => svc.GetGroupMembersAsync(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            this.authContext = FakeHttpContext.GetAuthorizationHandlerContextForTeamMember();

            // Act
            await this.policyHandler.HandleAsync(this.authContext);

            // Assert
            Assert.IsFalse(this.authContext.HasSucceeded);
        }
    }
}
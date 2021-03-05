// <copyright file="MustBeAdminTeamMemberHandlerTest.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Tests.Authentication.PolicyHandlerTest
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Teams.Apps.DIConnect.Authentication;
    using Microsoft.Teams.Apps.DIConnect.Authentication.AuthenticationHelper;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// Test class for admin team member policy handler.
    /// </summary>
    [TestClass]
    public class MustBeAdminTeamMemberHandlerTest
    {
        private Mock<IMemberValidationHelper> memberValidationHelper;
        private MustBeAdminTeamMemberHandler policyHandler;

        private AuthorizationHandlerContext authContext;

        /// <summary>
        /// Initialize all test variables.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.memberValidationHelper = new Mock<IMemberValidationHelper>();
            this.policyHandler = new MustBeAdminTeamMemberHandler(this.memberValidationHelper.Object);
        }

        /// <summary>
        /// Validate auth handle for success.
        /// </summary>
        /// <returns><see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task ValidateHandleAsync_Succeed()
        {
            // Arrange
            this.memberValidationHelper
                .Setup(svc => svc.IsAdminTeamMemberAsync(It.IsAny<string>()))
                .ReturnsAsync(() => true);

            this.authContext = FakeHttpContext.GetAuthorizationHandlerContextForAdminTeamMember();

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

            this.authContext = FakeHttpContext.GetAuthorizationHandlerContextForAdminTeamMember();

            // Act
            await this.policyHandler.HandleAsync(this.authContext);

            // Assert
            Assert.IsFalse(this.authContext.HasSucceeded);
        }
    }
}
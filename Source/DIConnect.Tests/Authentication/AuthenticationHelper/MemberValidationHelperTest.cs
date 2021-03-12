// <copyright file="MemberValidationHelperTest.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Tests.Authentication.AuthenticationHelper
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.DIConnect.Authentication;
    using Microsoft.Teams.Apps.DIConnect.Authentication.AuthenticationHelper;
    using Microsoft.Teams.Apps.DIConnect.Common.Services;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.Teams;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// Test class for member validation helper.
    /// </summary>
    [TestClass]
    public class MemberValidationHelperTest
    {
        private Mock<ITeamMembersService> teamMemberService;
        private Mock<IAppSettingsService> appSettingService;
        private IOptions<AuthenticationOptions> options;
        private Mock<IMemoryCache> memoryCache;
        private Mock<ILogger<MemberValidationHelper>> logger;

        private MemberValidationHelper memberValidationHelper;

        /// <summary>
        /// Initialize all test variables.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.teamMemberService = new Mock<ITeamMembersService>();
            this.appSettingService = new Mock<IAppSettingsService>();
            this.options = Options.Create(new AuthenticationOptions());
            this.memoryCache = new Mock<IMemoryCache>();
            this.logger = new Mock<ILogger<MemberValidationHelper>>();

            this.memberValidationHelper = new MemberValidationHelper(
                this.options,
                this.appSettingService.Object,
                this.teamMemberService.Object,
                this.memoryCache.Object,
                this.logger.Object);
        }

        /// <summary>
        /// Success validation of admin team member.
        /// </summary>
        /// <returns><see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task IsAdmimTeamMember_Success()
        {
            // Arrange
            this.memoryCache
                .Setup(x => x.CreateEntry(It.IsAny<string>()))
                .Returns(Mock.Of<ICacheEntry>);

            this.appSettingService
                .Setup(svc => svc.GetServiceUrlAsync())
                .ReturnsAsync(() => "Https:");

            this.teamMemberService
                .Setup(svc => svc.GetMembersAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(AuthenticationTestData.userDataEntities));

            // Act
            var result = await this.memberValidationHelper.IsAdminTeamMemberAsync("123");

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Failure validation of admin team member.
        /// </summary>
        /// <returns><see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task IsAdmimTeamMember_Failure()
        {
            // Arrange
            this.memoryCache
                .Setup(x => x.CreateEntry(It.IsAny<string>()))
                .Returns(Mock.Of<ICacheEntry>);

            this.appSettingService
                .Setup(svc => svc.GetServiceUrlAsync())
                .ReturnsAsync(() => "Https:");

            this.teamMemberService
                .Setup(svc => svc.GetMembersAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(AuthenticationTestData.userDataEntities));

            // Act
            var result = await this.memberValidationHelper.IsAdminTeamMemberAsync("897");

            // Assert
            Assert.IsFalse(result);
        }
    }
}
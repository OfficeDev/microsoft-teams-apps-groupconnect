// <copyright file="SyncPairUpMembersActivityTest.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Prep.Func.Test.PreparePairUpMatchesToSend.Activities
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Identity.Client;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.EmployeeResourceGroup;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.TeamData;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.UserData;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.UserPairupMapping;
    using Microsoft.Teams.Apps.DIConnect.Common.Services;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.Teams;
    using Microsoft.Teams.Apps.DIConnect.Prep.Func.PreparePairUpMatchesToSend.Activities;
    using Moq;
    using Xunit;

    /// <summary>
    /// SyncPairUpMembersActivity test class.
    /// </summary>
    public class SyncPairUpMembersActivityTest
    {
        private readonly Mock<ITeamMembersService> memberService = new Mock<ITeamMembersService>();
        private readonly Mock<IAppSettingsService> appSettingsService = new Mock<IAppSettingsService>();
        private readonly Mock<ITeamUserPairUpMappingRepository> teamUserPairUpMappingRepository = new Mock<ITeamUserPairUpMappingRepository>();
        public static readonly IOptions<ConfidentialClientApplicationOptions> options = Options.Create(new ConfidentialClientApplicationOptions()
        {
            TenantId = "00000000-0000-0000-0000-000000000000",
        });

        private readonly Mock<IEmployeeResourceGroupRepository> employeeResourceGroupRepository = new Mock<IEmployeeResourceGroupRepository>();

        /// <summary>
        /// Consturctor tests.
        /// </summary>
        [Fact]
        public void SyncPairUpMembersActivityTestConstructorTest()
        {
            // Arrange
            Action action1 = () => new SyncPairUpMembersActivity(null /*memberService*/, this.appSettingsService.Object, this.teamUserPairUpMappingRepository.Object, options);
            Action action2 = () => new SyncPairUpMembersActivity(this.memberService.Object, null /*appSettingsService*/, this.teamUserPairUpMappingRepository.Object, options);
            Action action3 = () => new SyncPairUpMembersActivity(this.memberService.Object, this.appSettingsService.Object, null /*teamUserPairUpMappingRepository*/, options);
            Action action4 = () => new SyncPairUpMembersActivity(this.memberService.Object, this.appSettingsService.Object, this.teamUserPairUpMappingRepository.Object, null);
            Action action5 = () => new SyncPairUpMembersActivity(this.memberService.Object, this.appSettingsService.Object, this.teamUserPairUpMappingRepository.Object, options);

            // Act and Assert.
            action1.Should().Throw<ArgumentNullException>("memberService is null.");
            action2.Should().Throw<ArgumentNullException>("appSettingsService is null.");
            action3.Should().Throw<ArgumentNullException>("teamUserPairUpMappingRepository is null.");
            action3.Should().Throw<ArgumentNullException>("options is null.");
            action5.Should().NotThrow();
        }

        /// <summary>
        /// Syncs pair up members team user pair up mapping repository table.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task SyncPairUpMembersActivitySuccessTest()
        {
            // Arrange
            var syncPairUpMembersActivity = this.SyncPairUpMembersActivity();
            string partitionKey = "abc";
            string rowKey = "xyz";
            string teamId = "00000000-0000-0000-0000-000000000000";
            TeamDataEntity teamData = new TeamDataEntity()
            { TeamId = "00000000-0000-0000-0000-000000000000", ServiceUrl = "https://www.abc.com" };
            Mock<ILogger> logger = new Mock<ILogger>();

            EmployeeResourceGroupEntity employeeResourceGroupEntity = new EmployeeResourceGroupEntity()
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
                TeamId = "00000000-0000-0000-0000-000000000000",
            };

            IEnumerable<UserDataEntity> userDataEntities = new List<UserDataEntity>()
            {
                new UserDataEntity() { TenantId = options.Value.TenantId },
            };

            TeamUserPairUpMappingEntity teamUserPairUpMappingEntity = new TeamUserPairUpMappingEntity()
            {
                TeamId = teamId,
            };

            this.appSettingsService
                .Setup(x => x.GetServiceUrlAsync())
                .Returns(Task.FromResult("https://www.abc.com"));
            this.memberService
                .Setup(x => x.GetUsersAsync(teamData.TeamId, options.Value.TenantId, teamData.ServiceUrl))
                .ReturnsAsync(userDataEntities);
            this.teamUserPairUpMappingRepository
                .Setup(x => x.GetAsync(partitionKey, rowKey))
                .Returns(Task.FromResult(teamUserPairUpMappingEntity));
            this.teamUserPairUpMappingRepository
                .Setup(x => x.CreateOrUpdateAsync(It.IsAny<TeamUserPairUpMappingEntity>()))
                .Returns(Task.CompletedTask);

            // Act
            Func<Task> task = async () => await syncPairUpMembersActivity.RunAsync(employeeResourceGroupEntity, logger.Object);

            // Assert
            await task.Should().NotThrowAsync();
            this.appSettingsService.Verify(x => x.GetServiceUrlAsync());
            this.memberService.Verify(x => x.GetUsersAsync(It.Is<string>(x => x.Equals(teamData.TeamId)), It.Is<string>(x => x.Equals(options.Value.TenantId)), It.Is<string>(x => x.Equals(teamData.ServiceUrl))));
            this.teamUserPairUpMappingRepository.Verify(x => x.GetAsync(It.IsAny<string>(), It.Is<string>(x => x.Equals(teamId))));
            this.teamUserPairUpMappingRepository.Verify(x => x.CreateOrUpdateAsync(It.Is<TeamUserPairUpMappingEntity>(x => x.TeamId == teamId)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncPairUpMembersActivity"/> class.
        /// </summary>
        /// <returns>return the instance of SyncPairUpMembersActivity.</returns>
        private SyncPairUpMembersActivity SyncPairUpMembersActivity()
        {
            return new SyncPairUpMembersActivity(this.memberService.Object, this.appSettingsService.Object, this.teamUserPairUpMappingRepository.Object, options);
        }
    }
}
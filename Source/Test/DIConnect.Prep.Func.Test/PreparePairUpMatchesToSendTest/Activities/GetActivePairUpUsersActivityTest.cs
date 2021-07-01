// <copyright file="GetActivePairUpUsersActivityTest.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Prep.Func.Test.PreparePairUpMatchesToSend.Activities
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using Microsoft.Graph;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.EmployeeResourceGroup;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.UserPairupMapping;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.MicrosoftGraph;
    using Microsoft.Teams.Apps.DIConnect.Prep.Func.PreparePairUpMatchesToSend.Activities;
    using Moq;
    using Xunit;

    /// <summary>
    /// GetActivePairUpUsersActivity test class.
    /// </summary>
    public class GetActivePairUpUsersActivityTest
    {
        private readonly Mock<IUsersService> userService = new Mock<IUsersService>();
        private readonly Mock<ITeamUserPairUpMappingRepository> teamUserPairUpMappingRepository = new Mock<ITeamUserPairUpMappingRepository>();
        private readonly Mock<IEmployeeResourceGroupRepository> employeeResourceGroupRepository = new Mock<IEmployeeResourceGroupRepository>();

        /// <summary>
        /// Consturctor tests.
        /// </summary>
        [Fact]
        public void GetActivePairUpUsersActivityConstructorTest()
        {
            // Arrange
            Action action1 = () => new GetActivePairUpUsersActivity(null /*userService*/, this.teamUserPairUpMappingRepository.Object, this.employeeResourceGroupRepository.Object);
            Action action2 = () => new GetActivePairUpUsersActivity(this.userService.Object, null /*teamUserPairUpMappingRepository*/, this.employeeResourceGroupRepository.Object);
            Action action3 = () => new GetActivePairUpUsersActivity(this.userService.Object, this.teamUserPairUpMappingRepository.Object, null /*employeeResourceGroupRepository*/);
            Action action4 = () => new GetActivePairUpUsersActivity(this.userService.Object, this.teamUserPairUpMappingRepository.Object, this.employeeResourceGroupRepository.Object);

            // Act and Assert.
            action1.Should().Throw<ArgumentNullException>("userService is null.");
            action2.Should().Throw<ArgumentNullException>("teamUserPairUpMappingRepository is null.");
            action3.Should().Throw<ArgumentNullException>("employeeResourceGroupRepository is null.");
            action4.Should().NotThrow();
        }

        /// <summary>
        /// Gets list of team user mappings to be sent to service bus.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Fact]
        public async Task GetActivePairUpUsersActivitySuccessTest()
        {
            // Arrange
            var getActivePairUpUsersActivity = this.GetActivePairUpUsersActivity();

            string partitionKey = "abc";
            string rowKey = "xyz";
            var user = this.GetUser();
            Mock<ILogger> logger = new Mock<ILogger>();
            EmployeeResourceGroupEntity employeeResourceGroupEntity = new EmployeeResourceGroupEntity()
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
                TeamId = "00000000-0000-0000-0000-000000000000",
            };

            IEnumerable<TeamUserPairUpMappingEntity> teamUserPairUpMappingEntity = new List<TeamUserPairUpMappingEntity>()
            {
                new TeamUserPairUpMappingEntity()
                {
                    UserObjectId = "00000000-0000-0000-0000-000000000000",
                    TeamId = "00000000-0000-0000-0000-000000000000",
                    IsPaused = true,
                },
            };

            this.employeeResourceGroupRepository
                .Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(employeeResourceGroupEntity));
            this.teamUserPairUpMappingRepository
                .Setup(x => x.GetActivePairUpUsersAsync(employeeResourceGroupEntity.TeamId))
                .ReturnsAsync(teamUserPairUpMappingEntity);
            this.userService.Setup(x => x.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);

            // Act
            Func<Task> task = async () => await getActivePairUpUsersActivity.RunAsync(employeeResourceGroupEntity, logger.Object);

            // Assert
            await task.Should().NotThrowAsync();
            this.employeeResourceGroupRepository.Verify(x => x.GetAsync(partitionKey, rowKey));
            this.teamUserPairUpMappingRepository.Verify(x => x.GetActivePairUpUsersAsync(employeeResourceGroupEntity.TeamId));
            this.userService.Verify(x => x.GetUserAsync(It.IsAny<string>()), Times.Once());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetActivePairUpUsersActivity"/> class.
        /// </summary>
        /// <returns>return the instance of GetActivePairUpUsersActivity.</returns>
        private GetActivePairUpUsersActivity GetActivePairUpUsersActivity()
        {
            return new GetActivePairUpUsersActivity(this.userService.Object, this.teamUserPairUpMappingRepository.Object, this.employeeResourceGroupRepository.Object);
        }

        private User GetUser()
        {
            return new User()
            {
                DisplayName = "dummy",
                Id = "12",
                UserPrincipalName = "UserPrincipalName",
            };
        }
    }
}
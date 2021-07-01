// <copyright file="EmployeeResourceGroupControllerTest.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Test.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Graph;
    using Microsoft.Teams.Apps.DIConnect.Bot;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.EmployeeResourceGroup;
    using Microsoft.Teams.Apps.DIConnect.Common.Resources;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.MicrosoftGraph;
    using Microsoft.Teams.Apps.DIConnect.Controllers;
    using Moq;
    using Xunit;

    /// <summary>
    /// EmployeeResourceGroupController test class.
    /// </summary>
    public class EmployeeResourceGroupControllerTest
    {
        private readonly Mock<IEmployeeResourceGroupRepository> employeeResourceGroupRepository = new Mock<IEmployeeResourceGroupRepository>();
        private readonly Mock<TableRowKeyGenerator> tableRowKeyGenerator = new Mock<TableRowKeyGenerator>();
        private readonly Mock<IGroupMembersService> groupMembersService = new Mock<IGroupMembersService>();
        private readonly Mock<IOptions<BotFilterMiddlewareOptions>> options = new Mock<IOptions<BotFilterMiddlewareOptions>>();
        private readonly Mock<IStringLocalizer<Strings>> localizer = new Mock<IStringLocalizer<Strings>>();
        private readonly Mock<ILogger<EmployeeResourceGroupController>> logger = new Mock<ILogger<EmployeeResourceGroupController>>();

        /// <summary>
        /// Constructor test for all parameters.
        /// </summary>
        [Fact]
        public void EmployeeResourceGroupController_AllParameters_ShouldBeSuccess()
        {
            // Arrange
            Action action = () => new EmployeeResourceGroupController(this.employeeResourceGroupRepository.Object, this.tableRowKeyGenerator.Object, this.groupMembersService.Object, this.options.Object, this.localizer.Object, this.logger.Object);

            // Act and Assert.
            action.Should().NotThrow();
        }

        /// <summary>
        /// Constructor test for null parameters.
        /// </summary>
        [Fact]
        public void KnowledgeBaseSettingsController_NullParameter_ThrowsArgumentNullException()
        {
            // Arrange
            Action action1 = () => new EmployeeResourceGroupController(null /*employeeResourceGroupRepository*/, this.tableRowKeyGenerator.Object, this.groupMembersService.Object, this.options.Object, this.localizer.Object, this.logger.Object);
            Action action2 = () => new EmployeeResourceGroupController(this.employeeResourceGroupRepository.Object, null /*tableRowKeyGenerator*/, this.groupMembersService.Object, this.options.Object, this.localizer.Object, this.logger.Object);
            Action action3 = () => new EmployeeResourceGroupController(this.employeeResourceGroupRepository.Object, this.tableRowKeyGenerator.Object, null /*groupMembersService*/, this.options.Object, this.localizer.Object, this.logger.Object);
            Action action4 = () => new EmployeeResourceGroupController(this.employeeResourceGroupRepository.Object, this.tableRowKeyGenerator.Object, this.groupMembersService.Object, null /*options*/, this.localizer.Object, this.logger.Object);
            Action action5 = () => new EmployeeResourceGroupController(this.employeeResourceGroupRepository.Object, this.tableRowKeyGenerator.Object, this.groupMembersService.Object, this.options.Object, null /*localizer*/, this.logger.Object);
            Action action6 = () => new EmployeeResourceGroupController(this.employeeResourceGroupRepository.Object, this.tableRowKeyGenerator.Object, this.groupMembersService.Object, this.options.Object, this.localizer.Object, null /*logger*/);

            // Act and Assert.
            action1.Should().Throw<ArgumentNullException>("employeeResourceGroupRepository is null.");
            action2.Should().Throw<ArgumentNullException>("tableRowKeyGenerator is null.");
            action3.Should().Throw<ArgumentNullException>("groupMembersService is null.");
            action4.Should().Throw<ArgumentNullException>("options is null.");
            action5.Should().Throw<ArgumentNullException>("localizer is null.");
            action6.Should().Throw<ArgumentNullException>("logger is null.");
        }

        /// <summary>
        /// Get employee resource group data.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task GetEmployeeResourceGroupDataAsyncSuccessTest()
        {
            // Arrange
            var employeeResourceGroupController = this.GetEmployeeResourceGroupController();
            string partitionKey = "abc";
            string rowKey = "xyz";
            IEnumerable<EmployeeResourceGroupEntity> employeeResourceGroupEntity = new List<EmployeeResourceGroupEntity>()
            {
                new EmployeeResourceGroupEntity()
                {
                    PartitionKey = partitionKey,
                    RowKey = rowKey,
                    GroupType = 1,
                    GroupId = "00000000-0000-0000-0000-000000000000",
                    GroupName = "groupname",
                    GroupDescription = "groupdescription",
                    GroupLink = "grouplink",
                    ImageLink = "imagelink",
                    Tags = "tags",
                    Location = "location",
                },
            };

            this.employeeResourceGroupRepository
                .Setup(x => x.GetSearchableResourceGroupsAsync())
                .ReturnsAsync(employeeResourceGroupEntity);

            // Act
            Func<Task> task = async () => await employeeResourceGroupController.GetEmployeeResourceGroupDataAsync();

            // Assert
            await task.Should().NotThrowAsync();
            this.employeeResourceGroupRepository.Verify(x => x.GetSearchableResourceGroupsAsync(), Times.Once());
        }

        /// <summary>
        /// Create a new employee resource group.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task CreateEmployeeResourceGroupAsyncSuccessTest()
        {
            // Arrange
            var employeeResourceGroupController = this.GetEmployeeResourceGroupController();
            string partitionKey = "abcd";
            string rowKey = "pxyz";
            string grouplink = "grouplink";
            var groupId = "newgroupdid";
            string groupName = "groupname1";
            EmployeeResourceGroupEntity employeeResourceGroupEntity1 = null;

            IEnumerable<EmployeeResourceGroupEntity> employeeResourceGroupEntity2 = new List<EmployeeResourceGroupEntity>() { };
            IEnumerable<EmployeeResourceGroupEntity> employeeResourceGroupEntity = new List<EmployeeResourceGroupEntity>()
            {
                new EmployeeResourceGroupEntity()
                {
                    PartitionKey = partitionKey,
                    RowKey = rowKey,
                    GroupType = 2,
                    GroupId = "00000000-0000-0000-0000-000000000000",
                    GroupName = groupName,
                    GroupDescription = "groupdescription1",
                    GroupLink = grouplink,
                    ImageLink = "imagelink1",
                    Tags = "tags1",
                    Location = "location",
                    TeamId = "00000000-0000-1111-0000-000000000000",
                    IsProfileMatchingEnabled = true,
                    MatchingFrequency = 1,
                    CreatedByObjectId = "231331",
                },
            };

            var users = new List<User>()
            {
                new User() { Id = groupId },
            };

            var context = new Mock<HttpContext>();
            var user = new Mock<ClaimsPrincipal>();
            this.employeeResourceGroupRepository
                .Setup(x => x.GetFilterDataByGroupLinkOrGroupNameAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(employeeResourceGroupEntity2);
            this.groupMembersService
                .Setup(x => x.GetGroupMembersAsync(It.IsAny<string>()))
                .ReturnsAsync(users);
            this.tableRowKeyGenerator
                .Setup(x => x.CreateNewKeyOrderingOldestToMostRecent())
                .Returns("string");
            this.employeeResourceGroupRepository
                .Setup(x => x.CreateOrUpdateAsync(It.IsAny<EmployeeResourceGroupEntity>()))
                .Returns(Task.CompletedTask);

            context.Setup(ctx => ctx.User).Returns(user.Object);

            // Act
            var result = await employeeResourceGroupController.CreateEmployeeResourceGroupAsync(employeeResourceGroupEntity1);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        /// <summary>
        /// Get employee resource group.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task GetEmployeeResourceGroupSuccessTest()
        {
            // Arrange
            var employeeResourceGroupController = this.GetEmployeeResourceGroupController();
            string partitionKey = "Group";
            string rowKey = "xyz";
            string id = "00000000-0000-0000-0000-000000000000";
            EmployeeResourceGroupEntity employeeResourceGroupEntity = new EmployeeResourceGroupEntity()
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
                GroupType = 1,
                GroupId = id,
                GroupName = "groupname",
                GroupDescription = "groupdescription",
                GroupLink = "grouplink",
                ImageLink = "imagelink",
                Tags = "tags",
                Location = "location",
                IncludeInSearchResults = true,
                MatchingFrequency = 1,
                IsProfileMatchingEnabled = true,
            };

            ResourceGroupResponse resourceGroupResponse = new ResourceGroupResponse();

            this.employeeResourceGroupRepository
                .Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(employeeResourceGroupEntity);

            // Act
            Func<Task> task = async () => await employeeResourceGroupController.GetEmployeeResourceGroup(id);

            // Assert
            task.Should().NotThrowAsync();
            this.employeeResourceGroupRepository.Verify(x => x.GetAsync(partitionKey, id));
        }

        /// <summary>
        /// Get all employee resource groups
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task GetAllEmployeeResourceGroupsSuccessTest()
        {
            // Arrange
            var employeeResourceGroupController = this.GetEmployeeResourceGroupController();
            string partition = null;
            int? count = null;
            IEnumerable<EmployeeResourceGroupEntity> employeeResourceGroupEntity = new List<EmployeeResourceGroupEntity>()
            {
                new EmployeeResourceGroupEntity()
                {
                    GroupType = 2,
                    GroupId = "00000000-0000-0000-0000-000000000000",
                    GroupName = "groupname1",
                    GroupDescription = "groupdescription1",
                    GroupLink = "grouplink",
                    ImageLink = "imagelink1",
                    Tags = "tags1",
                    Location = "location",
                    TeamId = "00000000-0000-1111-0000-000000000000",
                    IsProfileMatchingEnabled = true,
                    MatchingFrequency = 1,
                    CreatedByObjectId = "231331",
                },
            };

            this.employeeResourceGroupRepository
                .Setup(x => x.GetAllAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(employeeResourceGroupEntity);

            // Act
            Func<Task> task = async () => await employeeResourceGroupController.GetAllEmployeeResourceGroups();

            // Assert
            task.Should().NotThrow();
            this.employeeResourceGroupRepository.Verify(x => x.GetAllAsync(partition, count));
        }

        /// <summary>
        /// Get employee resource group by team id.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task GetEmployeeResourceGroupByTeamIdSuccessTest()
        {
            // Arrange
            var employeeResourceGroupController = this.GetEmployeeResourceGroupController();
            string id = "00:00000000000000000000@abc.xyz2";
            string groupId = "00000000-0000-0000-0000-000000000000";
            string partitionKey = "Group";
            string rowKey = "xyz";
            EmployeeResourceGroupEntity employeeResourceGroupEntity = new EmployeeResourceGroupEntity()
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
                GroupType = 1,
                GroupId = groupId,
                GroupName = "groupname",
                GroupDescription = "groupdescription",
                GroupLink = "teams.microsoft.com/l/team/00%00000000-0000-0000-0000-000000000000%40thread.tacv2/conversations?groupId=53b4782c-0000-0000-993a-441870d10af9&tenantId=00000000-0000-0000-0000-000000000000",
                ImageLink = "imagelink",
                Tags = "tags",
                Location = "location",
                IncludeInSearchResults = true,
                MatchingFrequency = 1,
                IsProfileMatchingEnabled = true,
                TeamId = id,
            };

            this.employeeResourceGroupRepository
                .Setup(x => x.GetResourceGroupByTeamIdAsync(employeeResourceGroupEntity.TeamId))
                .ReturnsAsync(employeeResourceGroupEntity);

            // Act
            Func<Task> task = async () => await employeeResourceGroupController.GetEmployeeResourceGroupByTeamId(id, groupId);

            // Assert
            task.Should().NotThrow();
            this.employeeResourceGroupRepository.Verify(x => x.GetResourceGroupByTeamIdAsync(It.Is<string>(x => x.Equals(employeeResourceGroupEntity.TeamId))), Times.Once());
        }

        /// <summary>
        /// Update employee resource group.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task UpdateEmployeeResourceGroupAsyncSuccessTest()
        {
            // Arrange
            var employeeResourceGroupController = this.GetEmployeeResourceGroupController();
            string id = null;
            string groupId = null;
            string userId = "http://schemas.microsoft.com/identity/claims/objectidentifier";
            string resourceGroupTablePartitionKey = "Group";
            string grouplink = "https://teams.microsoft.com/l/team/00%00000000-0000-0000-0000-000000000000%40thread.tacv2/conversations?groupId=00000000-0000-0000-0000-000000000000&tenantId=00000000-0000-0000-0000-000000000000";

            ResourceGroupRequest resourceGroupRequest1 = null;
            EmployeeResourceGroupEntity employeeResourceGroupEntity = new EmployeeResourceGroupEntity()
            {
                GroupType = 3,
                GroupName = "Group",
                GroupDescription = "description",
                GroupLink = "teams.microsoft.com/l/team/00%00000000-0000-0000-0000-000000000000%40thread.tacv2/conversations?groupId=00000000-0000-0000-0000-000000000000&tenantId=00000000-0000-0000-0000-000000000000",
                ImageLink = "imagelink",
                Tags = "tags",
                Location = "location",
                IncludeInSearchResults = true,
                IsProfileMatchingEnabled = true,
                MatchingFrequency = 1,
                UpdatedByObjectId = userId,
                UpdatedOn = DateTime.UtcNow,
            };

            IEnumerable<EmployeeResourceGroupEntity> iEnumerableEmployeeResourceGroupEntity = new List<EmployeeResourceGroupEntity>()
            {
                new EmployeeResourceGroupEntity()
                {
                    PartitionKey = resourceGroupTablePartitionKey,
                    RowKey = id,
                    GroupType = 2,
                    GroupId = "00000000-0000-0000-0000-000000000000",
                    GroupName = "groupName",
                    GroupDescription = "groupdescription1",
                    GroupLink = grouplink,
                    ImageLink = "imagelink1",
                    Tags = "tags1",
                    Location = "location",
                    TeamId = "00000000-0000-1111-0000-000000000000",
                    IsProfileMatchingEnabled = true,
                    MatchingFrequency = 1,
                    CreatedByObjectId = "231331",
                },
            };

            var users = new List<User>()
            {
                new User() { Id = id },
            };

            this.employeeResourceGroupRepository
                .Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(employeeResourceGroupEntity);
            this.employeeResourceGroupRepository
                .Setup(x => x.GetFilterDataByGroupLinkOrGroupNameAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(iEnumerableEmployeeResourceGroupEntity);
            this.groupMembersService
               .Setup(x => x.GetGroupMembersAsync(It.IsAny<string>()))
               .ReturnsAsync(users);
            this.employeeResourceGroupRepository
                .Setup(x => x.InsertOrMergeAsync(It.IsAny<EmployeeResourceGroupEntity>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await employeeResourceGroupController.UpdateEmployeeResourceGroupAsync(id, resourceGroupRequest1, groupId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeResourceGroupController"/> class.
        /// </summary>
        /// <returns>return the instance of GetActivePairUpUsersActivity.</returns>
        private EmployeeResourceGroupController GetEmployeeResourceGroupController()
        {
            return new EmployeeResourceGroupController(this.employeeResourceGroupRepository.Object, this.tableRowKeyGenerator.Object, this.groupMembersService.Object, this.options.Object, this.localizer.Object, this.logger.Object);
        }
    }
}
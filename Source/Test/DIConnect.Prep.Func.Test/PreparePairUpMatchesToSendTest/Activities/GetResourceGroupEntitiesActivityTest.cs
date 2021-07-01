// <copyright file="GetResourceGroupEntitiesActivityTest.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Prep.Func.Test.PreparePairUpMatchesToSend.Activities
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.EmployeeResourceGroup;
    using Microsoft.Teams.Apps.DIConnect.Prep.Func.PreparePairUpMatchesToSend.Activities;
    using Moq;
    using Xunit;

    /// <summary>
    /// GetResourceGroupEntitiesActivity test class.
    /// </summary>
    public class GetResourceGroupEntitiesActivityTest
    {
        private readonly Mock<IEmployeeResourceGroupRepository> employeeResourceGroupRepository = new Mock<IEmployeeResourceGroupRepository>();

        /// <summary>
        /// Consturctor tests.
        /// </summary>
        [Fact]
        public void GetActivePairUpUsersActivityConstructorTest()
        {
            // Arrange
            Action action1 = () => new GetResourceGroupEntitiesActivity(this.employeeResourceGroupRepository.Object);
            Action action2 = () => new GetResourceGroupEntitiesActivity(null);

            // Act and Assert
            action1.Should().NotThrow();
            action2.Should().Throw<ArgumentNullException>("employeeResourceGroupRepository is null.");
        }

        /// <summary>
        /// Repository for employee resource group.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task GetResourceGroupEntitiesActivitySuccessTest()
        {
            // Arrange
            var getResourceGroupEntitiesActivity = this.GetResourceGroupEntityActivity();
            int matchingFrequency = 1;
            Mock<ILogger> logger = new Mock<ILogger>();

            IEnumerable<EmployeeResourceGroupEntity> employeeResourceGroupEntities = new List<EmployeeResourceGroupEntity>()
            {
                new EmployeeResourceGroupEntity(),
            };

            this.employeeResourceGroupRepository
                .Setup(x => x.GetResourceGroupsOptedForPairUpMatchesAsync(matchingFrequency))
                .ReturnsAsync(employeeResourceGroupEntities);

            // Act
            Func<Task> task = async () => await getResourceGroupEntitiesActivity.RunAsync(matchingFrequency.ToString(), logger.Object);

            // Assert
            task.Should().NotThrow();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetResourceGroupEntitiesActivity"/> class.
        /// </summary>
        /// <returns>return the instance of GetResourceGroupEntitiesActivity.</returns>
        private GetResourceGroupEntitiesActivity GetResourceGroupEntityActivity()
        {
            return new GetResourceGroupEntitiesActivity(this.employeeResourceGroupRepository.Object);
        }
    }
}
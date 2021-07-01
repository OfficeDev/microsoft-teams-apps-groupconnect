// <copyright file="SyncRecipientsAndSendBatchesToQueueOrchestratorTest.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Prep.Func.Test.PreparePairUpMatchesToSend.Orchestrators
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.EmployeeResourceGroup;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.MessageQueues.UserPairupQueue;
    using Microsoft.Teams.Apps.DIConnect.Prep.Func.PreparingToSend;
    using Moq;
    using Xunit;

    /// <summary>
    /// SyncRecipientsAndSendBatchesToQueueOrchestrator test class.
    /// </summary>
    public class SyncRecipientsAndSendBatchesToQueueOrchestratorTest
    {
        private readonly Mock<IDurableOrchestrationContext> mockContext = new Mock<IDurableOrchestrationContext>();
        private readonly Mock<ILogger> mockLogger = new Mock<ILogger>();

        /// <summary>
        /// Sync recipient and send batches to queue orchestration Success test.
        /// </summary>
        /// <returns>A task that represents the work queued to execut.</returns>
        [Fact]
        public async Task SyncRecipientsAndSendBatchesToQueueOrchestratorSuccessTest()
        {
            // Arrange
            EmployeeResourceGroupEntity employeeResourceGroupEntity = new EmployeeResourceGroupEntity()
            {
                GroupLink = "teams.microsoft.com/l/team/00%00000000-0000-0000-0000-000000000000%00abc.abcd2/",
                TeamId = "00000000-0000-0000-0000-000000000000",
                GroupId = "00000000-0000-0000-0000-000000000000",
                PartitionKey = "abs",
            };

            List<TeamUserMapping> teamUserMappings = new List<TeamUserMapping>()
            {
                new TeamUserMapping { TeamId = "00000000-0000-0000-0000-000000000000", TeamName = "abc", },
            };
            this.mockContext
                .Setup(x => x.IsReplaying)
                .Returns(false);
            this.mockContext
                .Setup(x => x.GetInput<EmployeeResourceGroupEntity>())
                .Returns(employeeResourceGroupEntity);
            this.mockContext
                .Setup(x => x.CallActivityWithRetryAsync(It.IsAny<string>(), It.IsAny<RetryOptions>(), employeeResourceGroupEntity))
                .Returns(Task.CompletedTask);
            this.mockContext
                .Setup(x => x.CallActivityWithRetryAsync<List<TeamUserMapping>>(It.IsAny<string>(), It.IsAny<RetryOptions>(), employeeResourceGroupEntity))
                .ReturnsAsync(teamUserMappings);
            this.mockContext
                .Setup(x => x.CallActivityWithRetryAsync(It.IsAny<string>(), It.IsAny<RetryOptions>(), It.IsAny<object>()))
                .Returns(Task.CompletedTask);

            // Act
            Func<Task> task = async () => await SyncRecipientsAndSendBatchesToQueueOrchestrator.RunOrchestrator(this.mockContext.Object, this.mockLogger.Object);

            // Assert
            await task.Should().NotThrowAsync<Exception>();
            this.mockContext.Verify(x => x.CallActivityWithRetryAsync(It.Is<string>(x => x.Equals(FunctionNames.SyncPairUpMembersActivity)), It.IsAny<RetryOptions>(), employeeResourceGroupEntity), Times.Once());
            this.mockContext.Verify(x => x.CallActivityWithRetryAsync<List<TeamUserMapping>>(It.Is<string>(x => x.Equals(FunctionNames.GetActivePairUpUsersActivity)), It.IsAny<RetryOptions>(), It.IsAny<object>()), Times.Once());
            this.mockContext.Verify(x => x.CallActivityWithRetryAsync(It.Is<string>(x => x.Equals(FunctionNames.SendPairUpMatchesActivity)), It.IsAny<RetryOptions>(), It.IsAny<object>()), Times.Once());
        }
    }
}
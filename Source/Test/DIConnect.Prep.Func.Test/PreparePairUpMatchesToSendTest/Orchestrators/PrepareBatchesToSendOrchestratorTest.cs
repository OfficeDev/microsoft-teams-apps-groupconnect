// <copyright file="PrepareBatchesToSendOrchestratorTest.cs" company="Microsoft">
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
    using Microsoft.Teams.Apps.DIConnect.Prep.Func.PreparingToSend;
    using Moq;
    using Xunit;

    /// <summary>
    /// PrepareBatchesToSendOrchestrator test class.
    /// </summary>
    public class PrepareBatchesToSendOrchestratorTest
    {
        private readonly Mock<IDurableOrchestrationContext> mockContext = new Mock<IDurableOrchestrationContext>();
        private readonly Mock<ILogger> mockLogger = new Mock<ILogger>();

        /// <summary>
        /// Prepare batches to send orchestrator success Test.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [Fact]
        public async Task PrepareBatchesToSendOrchestratorSuccessTest()
        {
            // Arrange
            Mock<EmployeeResourceGroupEntity> mockEmployeeResourceGroupEntity = new Mock<EmployeeResourceGroupEntity>();
            IEnumerable<EmployeeResourceGroupEntity> employeeResourceGroupEntity = new List<EmployeeResourceGroupEntity>()
            {
                new EmployeeResourceGroupEntity(),
            };

            this.mockContext
                .Setup(x => x.IsReplaying)
                .Returns(false);
            this.mockContext
                .Setup(x => x.CallActivityWithRetryAsync<IEnumerable<EmployeeResourceGroupEntity>>(It.IsAny<string>(), It.IsAny<RetryOptions>(), It.IsAny<EmployeeResourceGroupEntity>()))
                .ReturnsAsync(employeeResourceGroupEntity);
            this.mockContext
                .Setup(x => x.CallSubOrchestratorWithRetryAsync(It.IsAny<string>(), It.IsAny<RetryOptions>(), employeeResourceGroupEntity))
                .Returns(Task.CompletedTask);

            // Act
            Func<Task> task = async () => await PrepareBatchesToSendOrchestrator.RunOrchestrator(this.mockContext.Object, this.mockLogger.Object);

            // Assert
            await task.Should().NotThrowAsync<Exception>();
            this.mockContext.Verify(x => x.CallActivityWithRetryAsync<IEnumerable<EmployeeResourceGroupEntity>>(It.Is<string>(x => x.Equals(FunctionNames.GetResourceGroupEntitiesActivity)), It.IsAny<RetryOptions>(), It.IsAny<object>()), Times.Once());
            this.mockContext.Verify(x => x.CallSubOrchestratorWithRetryAsync(It.Is<string>(x => x.Equals(FunctionNames.SyncRecipientsAndSendBatchesToQueueOrchestrator)), It.IsAny<RetryOptions>(), It.IsAny<EmployeeResourceGroupEntity>()), Times.Once());
        }
    }
}
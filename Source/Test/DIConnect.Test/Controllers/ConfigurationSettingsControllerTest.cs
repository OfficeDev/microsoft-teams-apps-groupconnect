// <copyright file="ConfigurationSettingsControllerTest.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Test.Controllers
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories;
    using Microsoft.Teams.Apps.DIConnect.Controllers;
    using Microsoft.Teams.Apps.DIConnect.Models;
    using Moq;
    using Xunit;

    /// <summary>
    /// ConfigurationSettingsController test.
    /// </summary>
    public class ConfigurationSettingsControllerTest
    {
        private readonly Mock<IAppConfigRepository> appConfigRepository = new Mock<IAppConfigRepository>();
        private readonly Mock<ILogger<ConfigurationSettingsController>> logger = new Mock<ILogger<ConfigurationSettingsController>>();

        /// <summary>
        /// Constructor test for all parameters.
        /// </summary>
        [Fact]
        public void ConfigurationSettingsController_AllParameters_ShouldBeSuccess()
        {
            // Arrange
            Action action = () => new ConfigurationSettingsController(this.appConfigRepository.Object,this.logger.Object);

            // Act and Assert.
            action.Should().NotThrow();
        }

        /// <summary>
        /// Constructor test for null parameters.
        /// </summary>
        [Fact]
        public void ConfigurationSettingsController_NullParameter_ThrowsArgumentNullException()
        {
            // Arrange
            Action action1 = () => new ConfigurationSettingsController(null /*appConfigRepository*/, this.logger.Object);
            Action action2 = () => new ConfigurationSettingsController(this.appConfigRepository.Object, null /*logger*/);

            // Act and Assert.
            action1.Should().Throw<ArgumentNullException>("appConfigRepository is null.");
            action2.Should().Throw<ArgumentNullException>("logger is null.");
        }

        /// <summary>
        /// Get faq configuration from the storage.
        /// </summary>
        /// <returns>FAQ configuration entity.</returns>
        [Fact]
        public async Task GetFaqConfigurationSuccessTest()
        {
            // Arrange
            var getConfigurationSettingsController = this.GetConfigurationSettingsController();
            string partitionKey = "Settings";
            string rowKey = "KnowledgeBaseId";
            string value = "value";
            AppConfigEntity appConfigEntity = new AppConfigEntity()
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
                Value = value,
            };
            this.appConfigRepository.Setup(x => x.GetAsync(partitionKey, rowKey)).ReturnsAsync(appConfigEntity);

            // Act
            Func<Task> task = async () => await getConfigurationSettingsController.GetFaqConfiguration();

            // Assert
            await task.Should().NotThrowAsync();
            Assert.Equal(appConfigEntity.Value, appConfigEntity.Value);
        }

        /// <summary>
        /// Update FAQ and group Configuration into storage.
        /// </summary>
        /// <returns>Update configuration.</returns>
        [Fact]
        public async Task UpdateConfigurationSuccessTest()
        {
            // Arrange
            var getConfigurationSettingsController = this.GetConfigurationSettingsController();
            string partitionKey = "Settings";
            string rowKey = "KnowledgeBaseId";
            string value = "value";
            var id = new ConfigurationData() { QnAMakerKnowledgeBaseId = "id" };
            AppConfigEntity appConfigEntity = new AppConfigEntity()
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
                Value = value,
            };
            this.appConfigRepository.Setup(x => x.GetAsync(partitionKey, rowKey)).ReturnsAsync(appConfigEntity);

            // Act
            var result = await getConfigurationSettingsController.UpdateConfiguration(id);

            // Assert
            Assert.Equal(id.QnAMakerKnowledgeBaseId, id.QnAMakerKnowledgeBaseId);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetConfigurationSettingsController"/> class.
        /// </summary>
        /// <returns>return the instance of GetActivePairUpUsersActivity.</returns>
        private ConfigurationSettingsController GetConfigurationSettingsController()
        {
            return new ConfigurationSettingsController(this.appConfigRepository.Object, this.logger.Object);
        }
    }
}
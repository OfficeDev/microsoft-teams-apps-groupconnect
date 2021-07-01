// <copyright file="ConfigurationSettingsController.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.DIConnect.Authentication;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories;
    using Microsoft.Teams.Apps.DIConnect.Models;

    /// <summary>
    /// Controller to handle configuration settings data operations.
    /// </summary>
    [Route("api/configurationSettings")]
    [Authorize]
    public class ConfigurationSettingsController : ControllerBase
    {
        /// <summary>
        /// Repository for app config data activity.
        /// </summary>
        private readonly IAppConfigRepository appConfigRepository;

        /// <summary>
        /// Sends logs to the telemetry service.
        /// </summary>
        private readonly ILogger<ConfigurationSettingsController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationSettingsController"/> class.
        /// </summary>
        /// <param name="appConfigRepository">Repository for app config data activity.</param>
        /// <param name="logger">Logs errors and information.</param>
        public ConfigurationSettingsController(
            IAppConfigRepository appConfigRepository,
            ILogger<ConfigurationSettingsController> logger)
        {
            this.appConfigRepository = appConfigRepository ?? throw new ArgumentNullException(nameof(appConfigRepository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get faq configuration from the storage.
        /// </summary>
        /// <returns>FAQ configuration entity.</returns>
        [HttpGet("faqconfiguration")]
        [Authorize(PolicyNames.MustBeAdminTeamMemberPolicy)]
        public async Task<IActionResult> GetFaqConfiguration()
        {
            try
            {
                var faqConfigurationEntity = await this.appConfigRepository.GetAsync(AppConfigTableName.SettingsPartition, AppConfigTableName.FAQConfigurationRowKey);

                if (faqConfigurationEntity == null)
                {
                    this.logger.LogInformation("No faq settings found.");
                    return this.NotFound("No faq settings found.");
                }

                return this.Ok(faqConfigurationEntity);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while fetching faq settings.");
                throw;
            }
        }

        /// <summary>
        /// Get employee resource group configuration from the storage.
        /// </summary>
        /// <returns>Employee resource group configuration entity.</returns>
        [HttpGet]
        public async Task<IActionResult> GetERGConfiguration()
        {
            try
            {
                var ergConfigurationEntity = await this.appConfigRepository.GetAsync(AppConfigTableName.SettingsPartition, AppConfigTableName.ERGConfigurationRowKey);

                if (ergConfigurationEntity == null)
                {
                    this.logger.LogInformation("No ERG settings found.");
                    return this.NotFound("No ERG settings found.");
                }

                return this.Ok(ergConfigurationEntity);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while fetching ERG settings.");
                throw;
            }
        }

        /// <summary>
        /// Update FAQ and group Configuration into storage.
        /// </summary>
        /// <param name="configurationData">Configuration data.</param>
        /// <returns>Update configuration.</returns>
        [HttpPatch]
        [Authorize(PolicyNames.MustBeAdminTeamMemberPolicy)]
        public async Task<IActionResult> UpdateConfiguration([FromBody] ConfigurationData configurationData)
        {
            try
            {
                if (configurationData == null)
                {
                    this.logger.LogWarning("Configurable data is null.");
                    return this.BadRequest("Configurable data is null.");
                }

                await this.appConfigRepository.CreateOrUpdateAsync(this.ConvertToConfigEntity(
                    AppConfigTableName.FAQConfigurationRowKey,
                    configurationData.QnAMakerKnowledgeBaseId,
                    configurationData.IsQnAEnabled));

                await this.appConfigRepository.CreateOrUpdateAsync(this.ConvertToConfigEntity(
                        AppConfigTableName.ERGConfigurationRowKey,
                        configurationData.RegisterERGButtonDisplayText,
                        configurationData.IsERGCreationRestrictedToGlobalTeam));

                return this.Ok();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while updating configuration of faq and ERG.");
                throw;
            }
        }

        /// <summary>
        /// Creates AppConfigEntity from configuration data values.
        /// </summary>
        /// <param name="rowkey">Rowkey of the config entity.</param>
        /// <param name="value">value of the config entity.</param>
        /// <param name="isEnabled">Configuration enabled status of the config entity.</param>
        /// <returns><see cref="AppConfigEntity"/> object.</returns>
        private AppConfigEntity ConvertToConfigEntity(string rowkey, string value, bool isEnabled)
        {
            return new AppConfigEntity
            {
                PartitionKey = AppConfigTableName.SettingsPartition,
                RowKey = rowkey,
                Value = value,
                IsEnabled = isEnabled,
            };
        }
    }
}
// <copyright file="KnowledgeBaseSettingsController.cs" company="Microsoft Corporation">
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
    /// Controller to handle knowledge base data operations.
    /// </summary>
    [Route("api/knowledgebase")]
    [Authorize(PolicyNames.MustBeAdminTeamMemberPolicy)]
    public class KnowledgeBaseSettingsController : ControllerBase
    {
        /// <summary>
        /// Repository for app config data activity.
        /// </summary>
        private readonly AppConfigRepository appConfigRepository;

        /// <summary>
        /// Sends logs to the telemetry service.
        /// </summary>
        private readonly ILogger<KnowledgeBaseSettingsController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="KnowledgeBaseSettingsController"/> class.
        /// </summary>
        /// <param name="appConfigRepository">Repository for app config data activity.</param>
        /// <param name="logger">Logs errors and information.</param>
        public KnowledgeBaseSettingsController(
            AppConfigRepository appConfigRepository,
            ILogger<KnowledgeBaseSettingsController> logger)
        {
            this.appConfigRepository = appConfigRepository ?? throw new ArgumentNullException(nameof(appConfigRepository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get knowledge base id from the storage.
        /// </summary>
        /// <returns>Knowledge base id.</returns>
        [HttpGet]
        public async Task<IActionResult> GetKnowledgeBaseId()
        {
            try
            {
                var knowledgeBaseEntity = await this.appConfigRepository.GetAsync(AppConfigTableName.SettingsPartition, AppConfigTableName.KnowledgeBaseIdRowKey);

                if (knowledgeBaseEntity == null)
                {
                    this.logger.LogInformation("No knowledge base detail found.");
                    return this.NotFound("No knowledge base detail found.");
                }

                return this.Ok(knowledgeBaseEntity.Value);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while fetching knowledge base id.");
                throw;
            }
        }

        /// <summary>
        /// Update knowledge base id into storage.
        /// </summary>
        /// <param name="knowledgeBaseData">QnA maker knowledge base data.</param>
        /// <returns>Update knowledge base id.</returns>
        [HttpPatch]
        public async Task<IActionResult> UpdateKnowledgeBaseId([FromBody]KnowledgeBaseData knowledgeBaseData)
        {
            try
            {
                if (string.IsNullOrEmpty(knowledgeBaseData.Id))
                {
                    this.logger.LogWarning("Request knowledge base id parsed as null or empty.");
                    return this.NotFound("Request knowledge base id cannot be null or empty.");
                }

                await this.appConfigRepository.CreateOrUpdateAsync(new AppConfigEntity
                {
                    PartitionKey = AppConfigTableName.SettingsPartition,
                    RowKey = AppConfigTableName.KnowledgeBaseIdRowKey,
                    Value = knowledgeBaseData.Id,
                });

                return this.Ok();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while updating knowledge base id.");
                throw;
            }
        }
    }
}
// <copyright file="PrepareBatchesToSendOrchestrator.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Prep.Func.PreparingToSend
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.EmployeeResourceGroup;

    /// <summary>
    /// Prepare batches to send orchestrator.
    /// This function prepares pair up batches and sends it to the queue.
    ///
    /// Performs following:
    /// 1. Fetch all the employee resource group entities based on matching frequency.
    /// 2. Starts sub-orchestrator to perform activities (sync recipients and send pair-up matches to queue).
    /// </summary>
    public static class PrepareBatchesToSendOrchestrator
    {
        /// <summary>
        /// This is the durable orchestration method,
        /// which kicks off the preparing to send process.
        /// </summary>
        /// <param name="context">Durable orchestration context.</param>
        /// <param name="log">Logging service.</param>
        /// <returns><see cref="Task"/> representing the asynchronous operation.</returns>
        [FunctionName(FunctionNames.PrepareBatchesToSendOrchestrator)]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            var matchingFrequnecy = context.InstanceId;

            if (!context.IsReplaying)
            {
                log.LogInformation($"Start to prepare pair up recipients for resource group entities based on frequency :{matchingFrequnecy}!");
            }

            try
            {
                if (!context.IsReplaying)
                {
                    log.LogInformation("About to fetch resource group entities.");
                }

                // fetch all resource groups based on matching frequency.
                var resourceGroupEntities = await context.CallActivityWithRetryAsync<IEnumerable<EmployeeResourceGroupEntity>>(
                    FunctionNames.GetResourceGroupEntitiesActivity,
                    FunctionSettings.DefaultRetryOptions,
                    matchingFrequnecy);

                if (resourceGroupEntities == null || resourceGroupEntities.Count() == 0)
                {
                    log.LogInformation("Resource group entities not found as per the matching frequency");
                    return;
                }

                log.LogInformation($"About to process {resourceGroupEntities.Count()} resource group entities.");

                foreach (var entity in resourceGroupEntities)
                {
                    try
                    {
                        if (!context.IsReplaying)
                        {
                            log.LogInformation("About to sync and send pair up batches to queue.");
                        }

                        // start sub-orchestrator to sync recipients and send pair-up matches to queue.
                        await context.CallSubOrchestratorWithRetryAsync(
                                FunctionNames.SyncRecipientsAndSendBatchesToQueueOrchestrator,
                                FunctionSettings.DefaultRetryOptions,
                                entity);

                        log.LogInformation($"Successfully send pair up batches to queue for team: {entity.TeamId}.");
                    }
                    catch (Exception ex)
                    {
                        log.LogInformation($"Unable to send pair up batches to queue for team :{entity.TeamId} {ex.Message}.");
                    }
                }

                log.LogInformation($"PrepareBatchesToSendOrchestrator successfully completed for resource groups of matching frequency: {matchingFrequnecy}!");
            }
            catch (Exception ex)
            {
                var errorMessage = $"PrepareBatchesToSendOrchestrator failed for resource groups of matching frequency: {matchingFrequnecy} Exception Message: {ex.Message}!";
                log.LogError(ex, errorMessage);
            }
        }
    }
}
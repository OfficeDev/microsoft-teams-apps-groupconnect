// <copyright file="SyncRecipientsAndSendBatchesToQueueOrchestrator.cs" company="Microsoft Corporation">
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
    using Microsoft.Teams.Apps.DIConnect.Common.Extensions;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.EmployeeResourceGroup;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.MessageQueues.UserPairupQueue;

    /// <summary>
    /// Sync pair up recipients and Send batches to queue orchestrator.
    ///
    /// Does following:
    /// 1. Sync pair up recipients to Team user pair up mapping repository table.
    /// 2. Get active pair up users and  based on resource group entity.
    /// 3. Sends messages to user pair up Queue in batches.
    /// </summary>
    public static class SyncRecipientsAndSendBatchesToQueueOrchestrator
    {
        /// <summary>
        /// Sync pair up recipients and Send batches to queue orchestrator.
        ///
        /// Does following:
        /// 1. Reads all active users based on resource group entity.
        /// 2. Get active pair-up mappings to be sent to service bus.
        /// 3. Sends messages to user pair up Queue in batches.
        /// </summary>
        /// <param name="context">Durable orchestration context.</param>
        /// <param name="log">Logger.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [FunctionName(FunctionNames.SyncRecipientsAndSendBatchesToQueueOrchestrator)]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            var groupEntity = context.GetInput<EmployeeResourceGroupEntity>();
            var teamId = ParseTeamIdExtension.GetTeamIdFromDeepLink(groupEntity.GroupLink);

            try
            {
                if (!context.IsReplaying)
                {
                    log.LogInformation("About to sync pair up members.");
                }

                // sync pair up recipients to Team user pair up mapping repository table.
                await context.CallActivityWithRetryAsync(
                    FunctionNames.SyncPairUpMembersActivity,
                    FunctionSettings.DefaultRetryOptions,
                    groupEntity);

                if (!context.IsReplaying)
                {
                    log.LogInformation("About to get all active pair up users.");
                }

                // get active pair-up mappings to be sent to service bus.
                var batchesToSend = await context.CallActivityWithRetryAsync<List<TeamUserMapping>>(
                    FunctionNames.GetActivePairUpUsersActivity,
                    FunctionSettings.DefaultRetryOptions,
                    groupEntity);

                if (batchesToSend != null && batchesToSend.Any())
                {
                    log.LogInformation($"About to process {batchesToSend.Count()} users for team {groupEntity.TeamId}.");

                    if (!context.IsReplaying)
                    {
                        log.LogInformation("About to send pair up batches to queue.");
                    }

                    // Send pair up user batches to queue.
                    await context.CallActivityWithRetryAsync(
                        FunctionNames.SendPairUpMatchesActivity,
                        FunctionSettings.DefaultRetryOptions,
                        (teamId, batchesToSend));
                }

                log.LogInformation($"SyncRecipientsAndSendBatchesToQueueOrchestrator successfully completed for team: {groupEntity.GroupId}!");
            }
            catch (Exception ex)
            {
                var errorMessage = $"SyncRecipientsAndSendBatchesToQueueOrchestrator failed for team: {groupEntity.TeamId}. Exception Message: {ex.Message}";
                log.LogError(ex, errorMessage);

                throw;
            }
        }
    }
}
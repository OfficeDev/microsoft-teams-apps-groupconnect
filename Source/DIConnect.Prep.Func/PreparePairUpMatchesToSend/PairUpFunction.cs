// <copyright file="PairUpFunction.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Prep.Func.PreparePairUpMatchesToSend
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.EmployeeResourceGroup;
    using Microsoft.Teams.Apps.DIConnect.Prep.Func.PreparingToSend;

    /// <summary>
    /// Azure Function App triggered on daily basis and prepares the data to be processed into queue.
    /// </summary>
    public class PairUpFunction
    {
        /// <summary>
        /// Azure Function App triggered on daily basis.
        /// </summary>
        /// <param name="myTimer">The timer schedule.</param>
        /// <param name="starter">Durable orchestration client.</param>
        /// <param name="log">The logger.</param>
        /// <returns>A <see cref="Task"/>Representing the asynchronous operation.</returns>
        [FunctionName(FunctionNames.PairUpFunction)]
        public async Task Run(
            [TimerTrigger("0 30 09 * * *")] TimerInfo myTimer,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            log.LogInformation($"DI Connect pair up function executed at: {DateTime.UtcNow}");

            if (DateTime.UtcNow.DayOfWeek == DayOfWeek.Monday)
            {
                // Start PrepareBatchesToSendOrchestrator function.
                string instanceId = await starter.StartNewAsync(
                    FunctionNames.PrepareBatchesToSendOrchestrator,
                    MatchingFrequency.Weekly.ToString());

                log.LogInformation($"Sending user pair-up matches on weekly basis with started orchestration of ID = '{instanceId}'.");
            }

            if (DateTime.UtcNow.Day == 1)
            {
                // Start PrepareBatchesToSendOrchestrator function.
                string instanceId = await starter.StartNewAsync(
                    FunctionNames.PrepareBatchesToSendOrchestrator,
                    MatchingFrequency.Monthly.ToString());

                log.LogInformation($"Sending user pair-up matches on monthly basis with started orchestration of ID = '{instanceId}'.");
            }
        }
    }
}
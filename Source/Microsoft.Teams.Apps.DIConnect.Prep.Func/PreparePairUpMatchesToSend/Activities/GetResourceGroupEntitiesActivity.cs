// <copyright file="GetResourceGroupEntitiesActivity.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Prep.Func.PreparePairUpMatchesToSend.Activities
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.EmployeeResourceGroup;
    using Microsoft.Teams.Apps.DIConnect.Prep.Func.PreparingToSend;

    /// <summary>
    /// Get list of employee resource group entities based on matching frequency.
    /// </summary>
    public class GetResourceGroupEntitiesActivity
    {
        /// <summary>
        /// Repository for employee resource group.
        /// </summary>
        private readonly EmployeeResourceGroupRepository employeeResourceGroupRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetResourceGroupEntitiesActivity"/> class.
        /// </summary>
        /// <param name="employeeResourceGroupRepository">The employee resource group repository.</param>
        public GetResourceGroupEntitiesActivity(
            EmployeeResourceGroupRepository employeeResourceGroupRepository)
        {
            this.employeeResourceGroupRepository = employeeResourceGroupRepository ?? throw new ArgumentNullException(nameof(employeeResourceGroupRepository));
        }

        /// <summary>
        /// Run the activity.
        /// Gets list of resource group entities based on matching frequency.
        /// </summary>
        /// <param name="matchingFrequency">Matching frequency.</param>
        /// <param name="log">Logging service.</param>
        /// <returns>A <see cref="Task"/>Representing the asynchronous operation.</returns>
        [FunctionName(FunctionNames.GetResourceGroupEntitiesActivity)]
        public async Task<IEnumerable<EmployeeResourceGroupEntity>> RunAsync(
            [ActivityTrigger] string matchingFrequency,
            ILogger log)
        {
            try
            {
                var frequency = Enum.Parse(typeof(MatchingFrequency), matchingFrequency);

                // Get resource group entities as per the matching frequency.
                var groupEntities = await this.employeeResourceGroupRepository.GetResourceGroupsOptedForPairUpMatchesAsync((int)frequency);

                if (groupEntities == null)
                {
                    log.LogInformation($"Resource group entities does not exist based on frequency :{matchingFrequency}");

                    return null;
                }

                return groupEntities;
            }
            catch (Exception ex)
            {
                log.LogError($"Error while fetching resource group entities based on matching frequency: {matchingFrequency} Exception message: {ex.Message}");
                throw;
            }
        }
    }
}
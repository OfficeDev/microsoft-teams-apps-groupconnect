// <copyright file="GetActivePairUpUsersActivity.cs" company="Microsoft Corporation">
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
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.UserPairupMapping;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.MessageQueues.UserPairupQueue;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.MicrosoftGraph;
    using Microsoft.Teams.Apps.DIConnect.Prep.Func.PreparingToSend;

    /// <summary>
    /// Get active pair-up mappings to be sent to service bus.
    /// </summary>
    public class GetActivePairUpUsersActivity
    {
        /// <summary>
        /// Users service.
        /// </summary>
        private readonly IUsersService usersService;

        /// <summary>
        /// Repository for employee resource group.
        /// </summary>
        private readonly EmployeeResourceGroupRepository employeeResourceGroupRepository;

        /// <summary>
        /// Repository for team user pair-up mapping.
        /// </summary>
        private readonly TeamUserPairUpMappingRepository teamUserPairUpMappingRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetActivePairUpUsersActivity"/> class.
        /// </summary>
        /// <param name="usersService">The users service.</param>
        /// <param name="teamUserPairUpMappingRepository">Team user pair-up mapping repository.</param>
        /// <param name="employeeResourceGroupRepository">The employee resource group repository.</param>
        public GetActivePairUpUsersActivity(
            IUsersService usersService,
            TeamUserPairUpMappingRepository teamUserPairUpMappingRepository,
            EmployeeResourceGroupRepository employeeResourceGroupRepository)
        {
            this.usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
            this.teamUserPairUpMappingRepository = teamUserPairUpMappingRepository ?? throw new ArgumentNullException(nameof(teamUserPairUpMappingRepository));
            this.employeeResourceGroupRepository = employeeResourceGroupRepository ?? throw new ArgumentNullException(nameof(employeeResourceGroupRepository));
        }

        /// <summary>
        /// Run the activity.
        /// Gets list of team user mappings to be sent to service bus.
        /// </summary>
        /// <param name="resourceGroupEntity">Resource group entity.</param>
        /// <param name="log">Logging service.</param>
        /// <returns>A <see cref="Task"/>Representing the asynchronous operation.</returns>
        [FunctionName(FunctionNames.GetActivePairUpUsersActivity)]
        public async Task<List<TeamUserMapping>> RunAsync(
            [ActivityTrigger] EmployeeResourceGroupEntity resourceGroupEntity,
            ILogger log)
        {
            try
            {
                var groupEntity = await this.employeeResourceGroupRepository.GetAsync(resourceGroupEntity.PartitionKey, resourceGroupEntity.GroupId);
                var teamUserMappings = new List<TeamUserMapping>();

                if (groupEntity == null)
                {
                    log.LogInformation($"Resource group does not exist :{resourceGroupEntity.GroupName}");

                    return null;
                }

                // Get active pair up users based on 'IsPaused' flag.
                var userMappings = await this.teamUserPairUpMappingRepository.GetActivePairUpUsersAsync(groupEntity.TeamId);
                foreach (var userMapping in userMappings)
                {
                    try
                    {
                        // Get user details.
                        var userData = await this.usersService.GetUserAsync(userMapping.UserObjectId);

                        // Entity for pair-up mappings to be sent to service bus.
                        teamUserMappings.Add(new TeamUserMapping()
                        {
                            UserGivenName = userData.DisplayName,
                            UserObjectId = userData.Id,
                            UserPrincipalName = userData.UserPrincipalName,
                            TeamId = groupEntity.TeamId,
                            TeamName = groupEntity.GroupName,
                        });
                    }
                    catch (Exception ex)
                    {
                       log.LogError($"Unable to fetch user details of user:{userMapping.UserObjectId} {ex.Message}");
                    }
                }

                return teamUserMappings;
            }
            catch (Exception ex)
            {
                log.LogError($"Error while creating pair-up mappings to be sent to service bus: {ex.Message} for Team: {resourceGroupEntity.TeamId}");
                throw;
            }
        }
    }
}
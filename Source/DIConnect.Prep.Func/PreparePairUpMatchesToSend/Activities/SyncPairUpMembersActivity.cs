// <copyright file="SyncPairUpMembersActivity.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Prep.Func.PreparePairUpMatchesToSend.Activities
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Identity.Client;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.EmployeeResourceGroup;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.UserPairupMapping;
    using Microsoft.Teams.Apps.DIConnect.Common.Services;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.Teams;
    using Microsoft.Teams.Apps.DIConnect.Prep.Func.PreparingToSend;

    /// <summary>
    /// Syncs pair up members to team user pair up mapping repository table.
    /// </summary>
    public class SyncPairUpMembersActivity
    {
        /// <summary>
        /// Team members service.
        /// </summary>
        private readonly ITeamMembersService memberService;

        /// <summary>
        /// App setting service.
        /// </summary>
        private readonly IAppSettingsService appSettingsService;

        /// <summary>
        /// Repository for team user pair-up mapping.
        /// </summary>
        private readonly TeamUserPairUpMappingRepository teamUserPairUpMappingRepository;

        /// <summary>
        /// A set of key/value application configuration properties for application settings.
        /// </summary>
        private readonly IOptions<ConfidentialClientApplicationOptions> options;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncPairUpMembersActivity"/> class.
        /// </summary>
        /// <param name="memberService">Teams member service.</param>
        /// <param name="appSettingsService">App settings service.</param>
        /// <param name="teamUserPairUpMappingRepository">Team user pair-up mapping repository.</param>
        /// <param name="options">A set of key/value application configuration properties for application settings.</param>
        public SyncPairUpMembersActivity(
            ITeamMembersService memberService,
            IAppSettingsService appSettingsService,
            TeamUserPairUpMappingRepository teamUserPairUpMappingRepository,
            IOptions<ConfidentialClientApplicationOptions> options)
        {
            this.memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
            this.appSettingsService = appSettingsService ?? throw new ArgumentNullException(nameof(appSettingsService));
            this.teamUserPairUpMappingRepository = teamUserPairUpMappingRepository ?? throw new ArgumentNullException(nameof(teamUserPairUpMappingRepository));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Syncs pair up members team user pair up mapping repository table.
        /// </summary>
        /// <param name="resourceGroupEntity">Input data.</param>
        /// <param name="log">Logging service.</param>
        /// <returns>A <see cref="Task"/>Representing the asynchronous operation.</returns>
        [FunctionName(FunctionNames.SyncPairUpMembersActivity)]
        public async Task RunAsync(
            [ActivityTrigger] EmployeeResourceGroupEntity resourceGroupEntity,
            ILogger log)
        {
            try
            {
                var serviceUrl = await this.appSettingsService.GetServiceUrlAsync();

                // Get team members.
                var userEntities = await this.memberService.GetMembersAsync(
                    teamId: resourceGroupEntity.TeamId,
                    tenantId: this.options.Value.TenantId,
                    serviceUrl: serviceUrl);

                foreach (var userEntity in userEntities)
                {
                    // Get user details from mapping storage table if already exists.
                    var userMapping = await this.teamUserPairUpMappingRepository.GetAsync(userEntity.AadId, resourceGroupEntity.TeamId);
                    if (userMapping == null)
                    {
                        var mappingEntity = new TeamUserPairUpMappingEntity
                        {
                            TeamId = resourceGroupEntity.TeamId,
                            UserObjectId = userEntity.AadId,
                            IsPaused = false,
                        };

                        log.LogInformation($"Inserting pair-up entity into table storage: {userEntity.AadId}");
                        await this.teamUserPairUpMappingRepository.CreateOrUpdateAsync(mappingEntity);
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Error while inserting pair-up matches: {ex.Message} for Team: {resourceGroupEntity.TeamId}");
                throw;
            }
        }
    }
}
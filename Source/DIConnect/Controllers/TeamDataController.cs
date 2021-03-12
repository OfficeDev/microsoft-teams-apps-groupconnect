// <copyright file="TeamDataController.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Graph;
    using Microsoft.Teams.Apps.DIConnect.Authentication;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.TeamData;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.MicrosoftGraph;
    using Microsoft.Teams.Apps.DIConnect.Models;

    /// <summary>
    /// Controller for the teams data.
    /// </summary>
    [Route("api/teamdata")]
    public class TeamDataController : ControllerBase
    {
        /// <summary>
        /// Instance of team data repository.
        /// </summary>
        private readonly TeamDataRepository teamDataRepository;

        /// <summary>
        /// Instance of Group data service.
        /// </summary>
        private readonly IGroupsService groupDataService;

        /// <summary>
        /// Instance to send logs to the logger service.
        /// </summary>
        private readonly ILogger<TeamDataController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamDataController"/> class.
        /// </summary>
        /// <param name="teamDataRepository">Team data repository instance.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="groupDataService">Group data service instance.</param>
        public TeamDataController(
            TeamDataRepository teamDataRepository,
            ILoggerFactory loggerFactory,
            IGroupsService groupDataService)
        {
            this.teamDataRepository = teamDataRepository ?? throw new ArgumentNullException(nameof(teamDataRepository));
            this.logger = loggerFactory?.CreateLogger<TeamDataController>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            this.groupDataService = groupDataService ?? throw new ArgumentNullException(nameof(groupDataService));
        }

        /// <summary>
        /// Get data for all teams.
        /// </summary>
        /// <returns>A list of team data.</returns>
        [HttpGet]
        [Authorize(PolicyNames.MustBeAdminTeamMemberPolicy)]
        public async Task<IEnumerable<TeamData>> GetAllTeamDataAsync()
        {
            var entities = await this.teamDataRepository.GetAllSortedAlphabeticallyByNameAsync();
            var result = new List<TeamData>();
            foreach (var entity in entities)
            {
                var team = new TeamData
                {
                    Id = entity.TeamId,
                    Name = entity.Name,
                };
                result.Add(team);
            }

            return result;
        }

        /// <summary>
        /// Get Teams details based on group Id.
        /// </summary>
        /// <param name="groupId">Group Id of the team.</param>
        /// <returns>Team data.</returns>
        [HttpGet("search")]
        [Authorize(PolicyNames.MustBeTeamMemberPolicy)]
        public async Task<ActionResult<TeamData>> GetTeamDataByGroupIdAsync([FromQuery] string groupId)
        {
            try
            {
                var entity = await this.groupDataService.GetTeamsInfoAsync(groupId);
                if (entity == null)
                {
                    this.logger.LogInformation($"No team details found for provided resource Id: {groupId}");
                    return this.NotFound($"No team details found for provided resource Id: {groupId}");
                }

                var result = new TeamData
                {
                    Name = entity.DisplayName,
                    Description = entity.Description,
                };

                return this.Ok(result);
            }
            catch (ServiceException ex)
            {
                this.logger.LogError($"Failed to fetch team details for group id: {groupId} - {ex.Message}");
                return this.BadRequest($"Failed to fetch team details for provided resource Id: {groupId}");
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Team data not found- {ex.Message}");
                return this.BadRequest($"Team data not found for provided resource Id: {groupId}");
            }
        }

        /// <summary>
        /// Method to verify team owner permission for given group id.
        /// </summary>
        /// <param name="groupId">Group Id of the team.</param>
        /// <returns>Returns true if given user is having owner permission for given team.</returns>
        [HttpGet("owner")]
        [Authorize(PolicyNames.MustBeTeamMemberPolicy)]
        public async Task<IActionResult> VerifyTeamsOwnerPermission([FromQuery] string groupId)
        {
            try
            {
                var teamOwnersList = await this.groupDataService.GetTeamOwnersAadObjectIdAsync(groupId);
                if (teamOwnersList == null || teamOwnersList.Count == 0)
                {
                    return this.Ok(false);
                }

                var userId = this.HttpContext.User.FindFirstValue(Common.Constants.ClaimTypeUserId);

                return this.Ok(teamOwnersList.Contains(userId));
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed to verify Teams access permission for group id:{groupId} - {ex.Message}");
                throw;
            }
        }
    }
}
// <copyright file="EmployeeResourceGroupController.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.DIConnect.Authentication;
    using Microsoft.Teams.Apps.DIConnect.Bot;
    using Microsoft.Teams.Apps.DIConnect.Common;
    using Microsoft.Teams.Apps.DIConnect.Common.Extensions;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.EmployeeResourceGroup;
    using Microsoft.Teams.Apps.DIConnect.Common.Resources;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.MicrosoftGraph;
    using Newtonsoft.Json;

    /// <summary>
    /// Controller to handle employee resource group operations.
    /// </summary>
    [Route("api/resourcegroups")]
    [Authorize]
    public class EmployeeResourceGroupController : ControllerBase
    {
        /// <summary>
        /// Repository for employee resource group activity.
        /// </summary>
        private readonly EmployeeResourceGroupRepository employeeResourceGroupRepository;

        /// <summary>
        /// Helper class to generate new row key.
        /// </summary>
        private readonly TableRowKeyGenerator tableRowKeyGenerator;

        /// <summary>
        /// Instance of group member service.
        /// </summary>
        private readonly IGroupMembersService groupMembersService;

        /// <summary>
        /// Bot filter middleware options.
        /// </summary>
        private readonly IOptions<BotFilterMiddlewareOptions> options;

        /// <summary>
        /// Sends logs to the telemetry service.
        /// </summary>
        private readonly ILogger<EmployeeResourceGroupController> logger;

        /// <summary>
        /// The current culture's string localizer.
        /// </summary>
        private readonly IStringLocalizer<Strings> localizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeResourceGroupController"/> class.
        /// </summary>
        /// <param name="employeeResourceGroupRepository">Employee resource group data repository instance.</param>
        /// <param name="tableRowKeyGenerator">Table row key generator service.</param>
        /// <param name="groupMembersService">Group member service.</param>
        /// <param name="options">Bot filter middleware options.</param>
        /// <param name="localizer">The current culture's string localizer.</param>
        /// <param name="logger">Logs errors and information.</param>
        public EmployeeResourceGroupController(
            EmployeeResourceGroupRepository employeeResourceGroupRepository,
            TableRowKeyGenerator tableRowKeyGenerator,
            IGroupMembersService groupMembersService,
            IOptions<BotFilterMiddlewareOptions> options,
            IStringLocalizer<Strings> localizer,
            ILogger<EmployeeResourceGroupController> logger)
        {
            this.employeeResourceGroupRepository = employeeResourceGroupRepository ?? throw new ArgumentNullException(nameof(employeeResourceGroupRepository));
            this.tableRowKeyGenerator = tableRowKeyGenerator ?? throw new ArgumentNullException(nameof(tableRowKeyGenerator));
            this.groupMembersService = groupMembersService ?? throw new ArgumentNullException(nameof(groupMembersService));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        /// <summary>
        /// Get call to retrieve searchable employee resource group data from Azure Search Service.
        /// </summary>
        /// <returns>List of searchable employee resource group entity.</returns>
        [HttpGet("discover")]
        public async Task<IActionResult> GetEmployeeResourceGroupDataAsync()
        {
            try
            {
                var groupEntities = await this.employeeResourceGroupRepository.GetSearchableResourceGroupsAsync();

                if (groupEntities == null)
                {
                    this.logger.LogInformation("No employee resource group entity found.");
                    return this.NotFound("No employee resource group entity found.");
                }

                var resourceGroupResponse = groupEntities.Select(
                    entity =>
                    {
                        return new ResourceGroupResponse()
                        {
                            GroupType = entity.GroupType,
                            GroupId = entity.GroupId,
                            GroupName = entity.GroupName,
                            GroupDescription = entity.GroupDescription,
                            GroupLink = entity.GroupLink,
                            ImageLink = entity.ImageLink,
                            Tags = JsonConvert.DeserializeObject<List<string>>(entity.Tags),
                            Location = entity.Location,
                        };
                    });

                return this.Ok(resourceGroupResponse);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while fetching employee resource group entities.");
                throw;
            }
        }

        /// <summary>
        /// Create a new employee resource group.
        /// </summary>
        /// <param name="employeeResourceGroupEntity">A new employee resource group entity to be created.</param>
        /// <returns>Create employee resource group.</returns>
        [HttpPost]
        public async Task<ActionResult<EmployeeResourceGroupEntity>> CreateEmployeeResourceGroupAsync([FromBody] EmployeeResourceGroupEntity employeeResourceGroupEntity)
        {
            try
            {
                if (employeeResourceGroupEntity == null)
                {
                    this.logger.LogWarning("Employee resource group entity is null.");
                    return this.BadRequest(this.localizer.GetString("ResourceGroupNullOrEmptyErrorMessage"));
                }

                // Storage call to get employee resource group entities if present already.
                var groupEntities = await this.employeeResourceGroupRepository.GetFilterDataByGroupLinkOrGroupNameAsync(
                    employeeResourceGroupEntity.GroupLink,
                    employeeResourceGroupEntity.GroupName);

                if (groupEntities.Any())
                {
                    this.logger.LogInformation($"Resource group entity already present with same group name {groupEntities.First().GroupName} or link :{groupEntities.First().GroupLink}.");
                    return this.BadRequest(this.localizer.GetString("GroupAlreadyExistsErrorMessage"));
                }

                this.logger.LogInformation("Initiated call to store employee resource group entity.");
                var userId = this.HttpContext.User.FindFirstValue(Constants.ClaimTypeUserId);

                // Validating if resource group type is 'Teams', user must be member of that group and they should belongs to same tenant.
                if (employeeResourceGroupEntity.GroupType == (int)ResourceGroupType.Teams)
                {
                    string tenantId = ParseTeamIdExtension.GetTenantIdFromDeepLink(employeeResourceGroupEntity.GroupLink);

                    if (!this.options.Value.AllowedTenants.Contains(tenantId))
                    {
                        this.logger.LogError($"Tenant is not valid: {tenantId}");
                        return this.BadRequest(this.localizer.GetString("InvalidTenantErrorMessage"));
                    }

                    string teamId = ParseTeamIdExtension.GetTeamIdFromDeepLink(employeeResourceGroupEntity.GroupLink);
                    string groupId = ParseTeamIdExtension.GetGroupIdFromDeepLink(employeeResourceGroupEntity.GroupLink);
                    var groupMembersDetail = await this.groupMembersService.GetGroupMembersAsync(groupId);
                    var groupMemberAadIds = groupMembersDetail.Select(row => row.Id).ToList();

                    if (!groupMemberAadIds.Contains(userId))
                    {
                        this.logger.LogError($"User {userId} is not a member of the team {teamId}");
                        return this.Forbid(this.localizer.GetString("InvalidGroupMemberErrorMessage"));
                    }

                    employeeResourceGroupEntity.TeamId = teamId;
                    employeeResourceGroupEntity.IsProfileMatchingEnabled = true;
                }
                else
                {
                    employeeResourceGroupEntity.IsProfileMatchingEnabled = false;
                }

                employeeResourceGroupEntity.Group = Constants.ResourceGroupTablePartitionKey;
                employeeResourceGroupEntity.GroupId = this.tableRowKeyGenerator.CreateNewKeyOrderingOldestToMostRecent();
                employeeResourceGroupEntity.ApprovalStatus = (int)ApprovalStatus.PendingForApproval;
                employeeResourceGroupEntity.CreatedOn = DateTime.UtcNow;
                employeeResourceGroupEntity.CreatedByObjectId = userId;
                employeeResourceGroupEntity.MatchingFrequency = (int)MatchingFrequency.Monthly;
                await this.employeeResourceGroupRepository.CreateOrUpdateAsync(employeeResourceGroupEntity);
                this.logger.LogInformation("Resource group - HTTP post call succeeded");

                return this.Created(this.Request.GetDisplayUrl(), employeeResourceGroupEntity);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while creating new employee resource group entity.");
                throw;
            }
        }

        /// <summary>
        /// Get employee resource group data entity for a given group id.
        /// </summary>
        /// <param name="id">Unique Id of resource group entity.</param>
        /// <returns>Resource group entity.</returns>
        [HttpGet("{id}")]
        [Authorize(PolicyNames.MustBeAdminTeamMemberPolicy)]
        public async Task<ActionResult<ResourceGroupResponse>> GetEmployeeResourceGroup(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    this.logger.LogWarning("Request id parsed as null or empty.");
                    return this.NotFound("Request id cannot be null or empty.");
                }

                var groupEntity = await this.employeeResourceGroupRepository.GetAsync(Constants.ResourceGroupTablePartitionKey, id);
                if (groupEntity == null)
                {
                    this.logger.LogInformation($"No record found for provided resource Id: {id}");
                    return this.NotFound($"No record found for provided resource Id : {id}");
                }

                var resourceGroupResponse = new ResourceGroupResponse
                {
                    GroupType = groupEntity.GroupType,
                    GroupId = groupEntity.GroupId,
                    GroupName = groupEntity.GroupName,
                    GroupDescription = groupEntity.GroupDescription,
                    GroupLink = groupEntity.GroupLink,
                    ImageLink = groupEntity.ImageLink,
                    Tags = JsonConvert.DeserializeObject<List<string>>(groupEntity.Tags),
                    Location = groupEntity.Location,
                    IncludeInSearchResults = groupEntity.IncludeInSearchResults,
                    MatchingFrequency = groupEntity.MatchingFrequency,
                    IsProfileMatchingEnabled = groupEntity.IsProfileMatchingEnabled,
                };

                return this.Ok(resourceGroupResponse);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while fetching employee resource group.");
                throw;
            }
        }

        /// <summary>
        /// Get all employee resource group data entities.
        /// </summary>
        /// <returns>List of employee resource group data entities.</returns>
        [HttpGet]
        [Authorize(PolicyNames.MustBeAdminTeamMemberPolicy)]
        public async Task<ActionResult<IEnumerable<ResourceGroupResponse>>> GetAllEmployeeResourceGroups()
        {
            try
            {
                var groupEntities = await this.employeeResourceGroupRepository.GetAllAsync();

                if (groupEntities == null)
                {
                    this.logger.LogInformation("No employee resource group entity found.");
                    return this.Ok(new List<ResourceGroupResponse>());
                }

                var resourceGroupResponse = groupEntities.Select(
                    entity =>
                    {
                        return new ResourceGroupResponse()
                        {
                            GroupType = entity.GroupType,
                            GroupId = entity.GroupId,
                            GroupName = entity.GroupName,
                            GroupDescription = entity.GroupDescription,
                            GroupLink = entity.GroupLink,
                            ImageLink = entity.ImageLink,
                            Tags = JsonConvert.DeserializeObject<List<string>>(entity.Tags),
                            Location = entity.Location,
                            IncludeInSearchResults = entity.IncludeInSearchResults,
                            MatchingFrequency = entity.MatchingFrequency,
                            IsProfileMatchingEnabled = entity.IsProfileMatchingEnabled,
                        };
                    });

                return this.Ok(resourceGroupResponse);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while fetching employee resource group entities.");
                throw;
            }
        }

        /// <summary>
        /// Get employee resource group data entity for a given team Id.
        /// </summary>
        /// <param name="teamId">Team id (19:xxx).</param>
        /// <param name="groupId">Group id of the team to fetch team members for user validation.</param>
        /// <returns>Resource group entity of type 'Teams'.</returns>
        [HttpGet("teams/{teamId}")]
        [Authorize(PolicyNames.MustBeTeamMemberPolicy)]
        public async Task<ActionResult<ResourceGroupResponse>> GetEmployeeResourceGroupByTeamId(string teamId, [FromQuery] string groupId)
        {
            try
            {
                var groupEntity = await this.employeeResourceGroupRepository.GetResourceGroupByTeamIdAsync(teamId);
                if (groupEntity == null)
                {
                    this.logger.LogInformation($"No record found for provided team Id: {teamId} and group Id: {groupId}.");
                    return this.NotFound($"No record found for provided team Id : {teamId} and group Id: {groupId}.");
                }

                if (groupId != ParseTeamIdExtension.GetGroupIdFromDeepLink(groupEntity.GroupLink))
                {
                    this.logger.LogInformation($"Group Id {groupId} and team Id {teamId} must belongs to same team.");
                    return this.BadRequest($"Group Id {groupId} and team Id {teamId} must belongs to same team.");
                }

                var resourceGroupResponse = new ResourceGroupResponse
                {
                    GroupType = groupEntity.GroupType,
                    GroupId = groupEntity.GroupId,
                    GroupName = groupEntity.GroupName,
                    GroupDescription = groupEntity.GroupDescription,
                    GroupLink = groupEntity.GroupLink,
                    ImageLink = groupEntity.ImageLink,
                    Tags = JsonConvert.DeserializeObject<List<string>>(groupEntity.Tags),
                    Location = groupEntity.Location,
                    IncludeInSearchResults = groupEntity.IncludeInSearchResults,
                    MatchingFrequency = groupEntity.MatchingFrequency,
                    IsProfileMatchingEnabled = groupEntity.IsProfileMatchingEnabled,
                };

                return this.Ok(resourceGroupResponse);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error while fetching employee resource group for team id: {teamId} and group Id: {groupId}.");
                throw;
            }
        }

        /// <summary>
        /// Update employee resource group entity.
        /// </summary>
        /// <param name="id">Id of resource group entity.</param>
        /// <param name="employeeResourceGroupEntity">A updated employee resource group entity to be updated.</param>
        /// <param name="groupId">Group id of the team to fetch team members for user validation.</param>
        /// <returns>Returns updated employee resource group entity.</returns>
        [HttpPatch("{id}")]
        [Authorize(PolicyNames.MustBeTeamOwnerOrAdminUserPolicy)]
        public async Task<ActionResult<EmployeeResourceGroupEntity>> UpdateEmployeeResourceGroupAsync(string id, [FromBody] ResourceGroupRequest employeeResourceGroupEntity, [FromQuery] string groupId)
        {
            try
            {
                if (employeeResourceGroupEntity == null)
                {
                    this.logger.LogError("Employee resource group entity is null.");
                    return this.BadRequest(this.localizer.GetString("ResourceGroupNullOrEmptyErrorMessage"));
                }

                var updateEntity = await this.employeeResourceGroupRepository.GetAsync(Constants.ResourceGroupTablePartitionKey, id);
                if (updateEntity == null)
                {
                    this.logger.LogError("The employee resource group entity that user is trying to update does not exist.");
                    return this.NotFound(this.localizer.GetString("ResourceGroupNotExistsErrorMessage"));
                }

                // Validate whether the updated link or name is already exists.
                if (updateEntity.GroupLink != employeeResourceGroupEntity.GroupLink || updateEntity.GroupName != employeeResourceGroupEntity.GroupName)
                {
                    // Storage call to get employee resource group entities if present already.
                    var groupEntities = await this.employeeResourceGroupRepository.GetFilterDataByGroupLinkOrGroupNameAsync(
                        employeeResourceGroupEntity.GroupLink,
                        employeeResourceGroupEntity.GroupName);

                    if (groupEntities.Any(entity => entity.GroupId != updateEntity.GroupId))
                    {
                        this.logger.LogInformation($"Resource group entity already present with same group name {groupEntities.First().GroupName} or link :{groupEntities.First().GroupLink}.");
                        return this.BadRequest(this.localizer.GetString("GroupAlreadyExistsErrorMessage"));
                    }
                }

                var userId = this.HttpContext.User.FindFirstValue(Constants.ClaimTypeUserId);

                // Validating if resource group type is 'Teams', user must be member of that group and they should belongs to same tenant.
                if (employeeResourceGroupEntity.GroupType == (int)ResourceGroupType.Teams)
                {
                    string tenantId = ParseTeamIdExtension.GetTenantIdFromDeepLink(employeeResourceGroupEntity.GroupLink);

                    if (!this.options.Value.AllowedTenants.Contains(tenantId))
                    {
                        this.logger.LogError($"Tenant is not valid: {tenantId}");
                        return this.BadRequest(this.localizer.GetString("InvalidTenantErrorMessage"));
                    }

                    string teamId = ParseTeamIdExtension.GetTeamIdFromDeepLink(employeeResourceGroupEntity.GroupLink);
                    var groupMembersDetail = await this.groupMembersService.GetGroupMembersAsync(groupId);
                    var groupMemberAadIds = groupMembersDetail.Select(row => row.Id).ToList();

                    if (!groupMemberAadIds.Contains(userId))
                    {
                        this.logger.LogError($"User {userId} is not a member of the team {teamId}");
                        return this.Forbid(this.localizer.GetString("InvalidGroupMemberErrorMessage"));
                    }

                    updateEntity.TeamId = teamId;
                }
                else
                {
                    updateEntity.TeamId = string.Empty;
                }

                updateEntity.GroupType = employeeResourceGroupEntity.GroupType;
                updateEntity.GroupName = employeeResourceGroupEntity.GroupName;
                updateEntity.GroupDescription = employeeResourceGroupEntity.GroupDescription;
                updateEntity.GroupLink = employeeResourceGroupEntity.GroupLink;
                updateEntity.ImageLink = employeeResourceGroupEntity.ImageLink;
                updateEntity.Tags = employeeResourceGroupEntity.Tags;
                updateEntity.Location = employeeResourceGroupEntity.Location;
                updateEntity.IncludeInSearchResults = employeeResourceGroupEntity.IncludeInSearchResults;
                updateEntity.IsProfileMatchingEnabled = employeeResourceGroupEntity.IsProfileMatchingEnabled;
                updateEntity.MatchingFrequency = employeeResourceGroupEntity.MatchingFrequency;
                updateEntity.UpdatedByObjectId = userId;
                updateEntity.UpdatedOn = DateTime.UtcNow;

                await this.employeeResourceGroupRepository.InsertOrMergeAsync(updateEntity);

                return this.Ok(updateEntity);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while updating employee resource group entity.");
                throw;
            }
        }

        /// <summary>
        /// Delete employee resource group entity.
        /// </summary>
        /// <param name="id">Id of resource group entity.</param>
        /// <returns>Returns http status code 200 for success call.</returns>
        [HttpDelete("{id}")]
        [Authorize(PolicyNames.MustBeAdminTeamMemberPolicy)]
        public async Task<ActionResult<EmployeeResourceGroupEntity>> DeleteEmployeeResourceGroupAsync(string id)
        {
            try
            {
                var groupEntity = await this.employeeResourceGroupRepository.GetAsync(Constants.ResourceGroupTablePartitionKey, id);
                if (groupEntity == null)
                {
                    return this.BadRequest($"Failed to delete, no record found for provided resource Id {id}.");
                }

                await this.employeeResourceGroupRepository.DeleteAsync(groupEntity);

                return this.Ok();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while deleting employee resource group entity.");
                throw;
            }
        }
    }
}
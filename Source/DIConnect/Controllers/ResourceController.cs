// <copyright file="ResourceController.cs" company="Microsoft Corporation">
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
    using Microsoft.Teams.Apps.DIConnect.Authentication;
    using Microsoft.Teams.Apps.DIConnect.Common;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.EmployeeResourceGroup;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.ResourceData;
    using Microsoft.Teams.Apps.DIConnect.Common.Resources;
    using Microsoft.Teams.Apps.DIConnect.DIConnect.Common.ResourceData;

    /// <summary>
    /// Controller to handle resource operations.
    /// </summary>
    [Route("api/resource")]
    [Authorize]
    public class ResourceController : ControllerBase
    {
        /// <summary>
        /// Repository for resource activity.
        /// </summary>
        private readonly IResourceDataRepository resourceDataRepository;

        /// <summary>
        /// Helper class to generate new row key.
        /// </summary>
        private readonly TableRowKeyGenerator tableRowKeyGenerator;

        /// <summary>
        /// Sends logs to the telemetry service.
        /// </summary>
        private readonly ILogger<ResourceController> logger;

        /// <summary>
        /// The current culture's string localizer.
        /// </summary>
        private readonly IStringLocalizer<Strings> localizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceController"/> class.
        /// </summary>
        /// <param name="resourceDataRepository">Resource data repository instance.</param>
        /// <param name="tableRowKeyGenerator">Table row key generator service.</param>
        /// <param name="logger">Logs errors and information.</param>
        /// <param name="localizer">The current culture's string localizer.</param>
        public ResourceController(
            IResourceDataRepository resourceDataRepository,
            TableRowKeyGenerator tableRowKeyGenerator,
            ILogger<ResourceController> logger,
            IStringLocalizer<Strings> localizer)
        {
            this.resourceDataRepository = resourceDataRepository ?? throw new ArgumentNullException(nameof(resourceDataRepository));
            this.tableRowKeyGenerator = tableRowKeyGenerator ?? throw new ArgumentNullException(nameof(tableRowKeyGenerator));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        /// <summary>
        /// Creates a new resource and stores in storage.
        /// </summary>
        /// <param name="resourceEntity">A new resource entity to be created.</param>
        /// <returns>Create resource.</returns>
        [HttpPost]
        [Authorize(PolicyNames.MustBeAdminTeamMemberPolicy)]
        public async Task<ActionResult<ResourceEntity>> CreateResourceAsync([FromBody] ResourceEntity resourceEntity)
        {
            try
            {
                if (resourceEntity == null)
                {
                    this.logger.LogWarning("Resource entity is null.");
                    return this.BadRequest(this.localizer.GetString("ResourceNullOrEmptyErrorMessage"));
                }

                // Storage call to get resource entities if present already.
                var resourceEntities = await this.resourceDataRepository.FindByRedirectionUrlOrTitleAsync(
                    resourceEntity.RedirectionUrl,
                    resourceEntity.ResourceTitle);

                if (resourceEntities.Any())
                {
                    this.logger.LogInformation($"Resource entity already present with same title {resourceEntities.First().ResourceTitle} or url :{resourceEntities.First().RedirectionUrl}.");
                    return this.BadRequest(this.localizer.GetString("ResourceAlreadyExistsErrorMessage"));
                }

                resourceEntity.Resource = Constants.ResourceTablePartitionKey;
                resourceEntity.ResourceId = this.tableRowKeyGenerator.CreateNewKeyOrderingMostRecentToOldest();
                resourceEntity.CreatedOn = DateTime.UtcNow;
                resourceEntity.CreatedByObjectId = this.HttpContext.User.FindFirstValue(Constants.ClaimTypeUserId);
                await this.resourceDataRepository.InsertOrMergeAsync(resourceEntity);

                this.logger.LogInformation("Resource group - HTTP post call succeeded");
                return this.Created(this.Request.GetDisplayUrl(), resourceEntity);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while creating new resource entity.");
                throw;
            }
        }

        /// <summary>
        /// Get resource entity for a given resource id.
        /// </summary>
        /// <param name="id">Unique Id of resource entity.</param>
        /// <returns>Resource entity.</returns>
        [HttpGet("{id}")]
        [Authorize(PolicyNames.MustBeAdminTeamMemberPolicy)]
        public async Task<ActionResult<ResourceEntity>> GetResourceDetailAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    this.logger.LogWarning("Request id parsed as null or empty.");
                    return this.NotFound("Request id cannot be null or empty.");
                }

                var resourceEntity = await this.resourceDataRepository.GetAsync(Constants.ResourceTablePartitionKey, id);
                if (resourceEntity == null)
                {
                    this.logger.LogInformation($"No record found for provided resource Id: {id}");
                    return this.NotFound($"No record found for provided resource Id : {id}");
                }

                var resourceResponse = new ResourceEntity
                {
                    ResourceType = resourceEntity.ResourceType,
                    ResourceId = resourceEntity.ResourceId,
                    ResourceTitle = resourceEntity.ResourceTitle,
                    ResourceDescription = resourceEntity.ResourceDescription,
                    ImageLink = resourceEntity.ImageLink,
                    RedirectionUrl = resourceEntity.RedirectionUrl,
                };

                return this.Ok(resourceResponse);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while fetching resource details from storage.");
                throw;
            }
        }

        /// <summary>
        /// Get all resource entities.
        /// </summary>
        /// <returns>List of resource entities.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResourceEntity>>> GetAllResourcesAsync()
        {
            try
            {
                var resourceEntities = await this.resourceDataRepository.GetAllAsync();

                if (resourceEntities == null)
                {
                    this.logger.LogInformation("No resource entity found.");
                    return this.Ok(new List<ResourceGroupResponse>());
                }

                var resourceGroupResponse = resourceEntities.Select(
                    entity =>
                    {
                        return new ResourceEntity()
                        {
                            ResourceType = entity.ResourceType,
                            ResourceId = entity.ResourceId,
                            ResourceTitle = entity.ResourceTitle,
                            ResourceDescription = entity.ResourceDescription,
                            ImageLink = entity.ImageLink,
                            RedirectionUrl = entity.RedirectionUrl,
                        };
                    });

                return this.Ok(resourceGroupResponse);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while fetching resource entities.");
                throw;
            }
        }

        /// <summary>
        /// Update resource entity.
        /// </summary>
        /// <param name="resourceEntity">Resource entity to be updated.</param>
        /// <returns>Returns updated resource entity.</returns>
        [HttpPatch]
        [Authorize(PolicyNames.MustBeAdminTeamMemberPolicy)]
        public async Task<ActionResult<ResourceEntity>> UpdateResourceAsync([FromBody] ResourceEntity resourceEntity)
        {
            try
            {
                if (resourceEntity == null)
                {
                    this.logger.LogError("Resource entity is null.");
                    return this.BadRequest(this.localizer.GetString("ResourceNullOrEmptyErrorMessage"));
                }

                var updateEntity = await this.resourceDataRepository.GetAsync(Constants.ResourceTablePartitionKey, resourceEntity.ResourceId);
                if (updateEntity == null)
                {
                    this.logger.LogError("The resource entity that user is trying to update does not exist.");
                    return this.NotFound(this.localizer.GetString("ResourceNotExistsErrorMessage"));
                }

                // Validate whether the updated url or table is already exists.
                if (updateEntity.RedirectionUrl != resourceEntity.RedirectionUrl || updateEntity.ResourceTitle != resourceEntity.ResourceTitle)
                {
                    // Storage call to get resource entities if present already.
                    var groupEntities = await this.resourceDataRepository.FindByRedirectionUrlOrTitleAsync(
                        resourceEntity.RedirectionUrl,
                        resourceEntity.ResourceTitle);

                    if (groupEntities.Any(entity => entity.ResourceId != updateEntity.ResourceId))
                    {
                        this.logger.LogInformation($"Resource entity already present with same title {groupEntities.First().ResourceTitle} or url :{groupEntities.First().RedirectionUrl}.");
                        return this.BadRequest(this.localizer.GetString("ResourceAlreadyExistsErrorMessage"));
                    }
                }

                updateEntity.ResourceTitle = resourceEntity.ResourceTitle;
                updateEntity.ResourceDescription = resourceEntity.ResourceDescription;
                updateEntity.RedirectionUrl = resourceEntity.RedirectionUrl;
                updateEntity.ImageLink = resourceEntity.ImageLink;
                updateEntity.ResourceType = resourceEntity.ResourceType;
                updateEntity.UpdatedByObjectId = this.HttpContext.User.FindFirstValue(Constants.ClaimTypeUserId);
                updateEntity.UpdatedOn = DateTime.UtcNow;

                await this.resourceDataRepository.InsertOrMergeAsync(updateEntity);

                return this.Ok(updateEntity);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while updating resource entity.");
                throw;
            }
        }

        /// <summary>
        /// Delete resource entity.
        /// </summary>
        /// <param name="id">Id of resource entity.</param>
        /// <returns>Returns http status code 200 for success call.</returns>
        [HttpDelete("{id}")]
        [Authorize(PolicyNames.MustBeAdminTeamMemberPolicy)]
        public async Task<ActionResult<ResourceEntity>> DeleteResourceDetailAsync(string id)
        {
            try
            {
                var resourceEntity = await this.resourceDataRepository.GetAsync(Constants.ResourceTablePartitionKey, id);
                if (resourceEntity == null)
                {
                    return this.BadRequest($"Failed to delete, no record found for provided resource Id {id}.");
                }

                await this.resourceDataRepository.DeleteAsync(resourceEntity);

                return this.Ok();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while deleting resource entity.");
                throw;
            }
        }
    }
}
// <copyright file="MustBeTeamOwnerOrAdminUserHandler.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Authentication
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.DIConnect.Authentication.AuthenticationHelper;
    using Microsoft.Teams.Apps.DIConnect.Common;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.MicrosoftGraph;

    /// <summary>
    /// This class is an authorization handler, which handles the authorization requirement.
    /// </summary>
    public class MustBeTeamOwnerOrAdminUserHandler : IAuthorizationHandler
    {
        /// <summary>
        /// Instance of MemberValidationService to validate member.
        /// </summary>
        private readonly IMemberValidationHelper memberValidationHelper;

        /// <summary>
        /// Service to fetch group details.
        /// </summary>
        private readonly IGroupsService groupsService;

        /// <summary>
        /// Cache for storing authorization result.
        /// </summary>
        private readonly IMemoryCache memoryCache;

        /// <summary>
        /// Instance to send logs to the logger service.
        /// </summary>
        private readonly ILogger<MustBeTeamOwnerOrAdminUserHandler> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MustBeTeamOwnerOrAdminUserHandler"/> class.
        /// </summary>
        /// <param name="memberValidationHelper">Instance of MemberValidationService to validate member.</param>
        /// <param name="groupsService">Groups service.</param>
        /// <param name="memoryCache">MemoryCache instance for caching authorization result.</param>
        /// <param name="logger">Logger implementation to send logs to the logger service.</param>
        public MustBeTeamOwnerOrAdminUserHandler(
            IMemberValidationHelper memberValidationHelper,
            IGroupsService groupsService,
            IMemoryCache memoryCache,
            ILogger<MustBeTeamOwnerOrAdminUserHandler> logger)
        {
            this.memberValidationHelper = memberValidationHelper ?? throw new ArgumentNullException(nameof(memberValidationHelper));
            this.groupsService = groupsService ?? throw new ArgumentNullException(nameof(groupsService));
            this.memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// This method handles the authorization requirement.
        /// </summary>
        /// <param name="context">AuthorizationHandlerContext instance.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        public async Task HandleAsync(AuthorizationHandlerContext context)
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            string teamLink = string.Empty;
            var oidClaim = context.User.Claims.FirstOrDefault(p => Constants.ClaimTypeUserId.Equals(p.Type, StringComparison.OrdinalIgnoreCase));

            foreach (var requirement in context.Requirements)
            {
                if (requirement is MustBeTeamOwnerOrAdminUserHandlerRequirement)
                {
                    if (context.Resource is AuthorizationFilterContext authorizationFilterContext)
                    {
                        // Wrap the request stream so that we can rewind it back to the start for regular request processing.
                        authorizationFilterContext.HttpContext.Request.EnableBuffering();

                        if (!string.IsNullOrEmpty(authorizationFilterContext.HttpContext.Request.QueryString.Value))
                        {
                            var requestQuery = authorizationFilterContext.HttpContext.Request.Query;
                            string groupId = requestQuery.Where(queryData => queryData.Key == "groupId").Select(queryData => queryData.Value.ToString()).FirstOrDefault();

                            // Check if current sign-in user is the owner of team.
                            if (await this.IsTeamOwnerAsync(groupId, oidClaim?.Value))
                            {
                                context.Succeed(requirement);
                            }
                        }

                        // Check if current sign-in user is the part of admin team.
                        if (await this.memberValidationHelper.IsAdminTeamMemberAsync(oidClaim.Value))
                        {
                            context.Succeed(requirement);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Check if a user is a owner of team.
        /// </summary>
        /// <param name="groupId">Group id of the team.</param>
        /// <param name="userAadObjectId">The user's Azure Active Directory object id.</param>
        /// <returns>The flag indicates that the user is a owner of certain team or not.</returns>
        private async Task<bool> IsTeamOwnerAsync(string groupId, string userAadObjectId)
        {
            try
            {
                bool isCacheEntryExists = this.memoryCache.TryGetValue(this.GetCacheKey(groupId, userAadObjectId), out bool isUserValidTeamOwner);

                if (!isCacheEntryExists)
                {
                    var ownerAadIds = await this.groupsService.GetTeamOwnersAadObjectIdAsync(groupId);

                    if (!ownerAadIds.Contains(userAadObjectId))
                    {
                        isUserValidTeamOwner = false;
                    }
                    else
                    {
                        isUserValidTeamOwner = true;
                    }

                    this.memoryCache.Set(this.GetCacheKey(groupId, userAadObjectId), isUserValidTeamOwner, TimeSpan.FromMinutes(Constants.CacheDurationInMinutes));
                }

                return isUserValidTeamOwner;
            }
#pragma warning disable CA1031 // Catching general exceptions to log exception details in telemetry client.
            catch (Exception ex)
#pragma warning restore CA1031 // Catching general exceptions to log exception details in telemetry client.
            {
                this.logger.LogError(ex, $"Error occurred while fetching team owner for team with group id: {groupId} - user object id: {userAadObjectId} ");

                // Return false if the member is not found in team id or either of the information is incorrect.
                // Caller should handle false value to throw unauthorized if required.
                return false;
            }
        }

        /// <summary>
        /// Get cache key value.
        /// </summary>
        /// <param name="groupId">Group id of the team.</param>
        /// <param name="userAadObjectId">Unique id of Azure Active Directory of user.</param>
        /// <returns>Returns a team cache key value.</returns>
        private string GetCacheKey(string groupId, string userAadObjectId)
        {
            return $"{groupId}{userAadObjectId}{PolicyNames.TeamOwnerCacheKey}";
        }
    }
}
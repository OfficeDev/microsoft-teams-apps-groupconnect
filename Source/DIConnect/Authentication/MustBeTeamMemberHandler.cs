// <copyright file="MustBeTeamMemberHandler.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Authentication
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.DIConnect.Common;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.MicrosoftGraph;

    /// <summary>
    /// This class is an authorization handler, which handles the authorization requirement.
    /// </summary>
    public class MustBeTeamMemberHandler : IAuthorizationHandler
    {
        /// <summary>
        /// Service to fetch group members details.
        /// </summary>
        private readonly IGroupMembersService groupMembersService;

        /// <summary>
        /// Cache for storing authorization result.
        /// </summary>
        private readonly IMemoryCache memoryCache;

        /// <summary>
        /// Instance to send logs to the logger service.
        /// </summary>
        private readonly ILogger<MustBeTeamMemberHandler> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MustBeTeamMemberHandler"/> class.
        /// </summary>
        /// <param name="groupMembersService">Group members service.</param>
        /// <param name="memoryCache">MemoryCache instance for caching authorization result.</param>
        /// <param name="logger">Logger implementation to send logs to the logger service.</param>
        public MustBeTeamMemberHandler(IGroupMembersService groupMembersService, IMemoryCache memoryCache, ILogger<MustBeTeamMemberHandler> logger)
        {
            this.groupMembersService = groupMembersService ?? throw new ArgumentNullException(nameof(groupMembersService));
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

            var oidClaim = context.User.Claims.FirstOrDefault(p => Constants.ClaimTypeUserId.Equals(p.Type, StringComparison.OrdinalIgnoreCase));

            foreach (var requirement in context.Requirements)
            {
                if (requirement is MustBeTeamMemberRequirement)
                {
                    if (context.Resource is AuthorizationFilterContext authorizationFilterContext)
                    {
                        // Wrap the request stream so that we can rewind it back to the start for regular request processing.
                        authorizationFilterContext.HttpContext.Request.EnableBuffering();

                        if (!string.IsNullOrEmpty(authorizationFilterContext.HttpContext.Request.QueryString.Value))
                        {
                            var requestQuery = authorizationFilterContext.HttpContext.Request.Query;
                            string groupId = requestQuery.Where(queryData => queryData.Key == "groupId").Select(queryData => queryData.Value.ToString()).FirstOrDefault();

                            // Check if current sign-in user is the part of team.
                            if (await this.IsTeamMemberAsync(groupId, oidClaim.Value))
                            {
                                context.Succeed(requirement);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Check if a user is a member of team.
        /// </summary>
        /// <param name="groupId">Group id of the team.</param>
        /// <param name="userAadObjectId">The user's Azure Active Directory object id.</param>
        /// <returns>The flag indicates that the user is a owner of certain team or not.</returns>
        private async Task<bool> IsTeamMemberAsync(string groupId, string userAadObjectId)
        {
            try
            {
                bool isCacheEntryExists = this.memoryCache.TryGetValue(this.GetCacheKey(groupId, userAadObjectId), out bool isUserValidTeamMember);

                if (!isCacheEntryExists)
                {
                    var groupMembersDetail = await this.groupMembersService.GetGroupMembersAsync(groupId);

                    if (groupMembersDetail == null || !groupMembersDetail.Where(row => row.Id == userAadObjectId).Any())
                    {
                        isUserValidTeamMember = false;
                    }
                    else
                    {
                        isUserValidTeamMember = true;
                    }

                    this.memoryCache.Set(this.GetCacheKey(groupId, userAadObjectId), isUserValidTeamMember, TimeSpan.FromMinutes(Constants.CacheDurationInMinutes));
                }

                return isUserValidTeamMember;
            }
#pragma warning disable CA1031 // Catching general exceptions to log exception details in telemetry client.
            catch (Exception ex)
#pragma warning restore CA1031 // Catching general exceptions to log exception details in telemetry client.
            {
                this.logger.LogError(ex, $"Error occurred while fetching team members based on group id: {groupId} - user object id: {userAadObjectId} ");

                // Return false if the member is not found in team id or either of the information is incorrect.
                // Caller should handle false value to throw unauthorized if required.
                return false;
            }
        }

        /// <summary>
        /// Get member cache key value.
        /// </summary>
        /// <param name="groupId">Group id of the team. </param>
        /// <param name="userAadObjectId">Unique id of Azure Active Directory of user.</param>
        /// <returns>Returns a team member cache key value.</returns>
        private string GetCacheKey(string groupId, string userAadObjectId)
        {
            return $"{groupId}{userAadObjectId}{PolicyNames.TeamMemberCacheKey}";
        }
    }
}
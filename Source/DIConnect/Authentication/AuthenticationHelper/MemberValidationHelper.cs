// <copyright file="MemberValidationHelper.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Authentication.AuthenticationHelper
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.DIConnect.Common;
    using Microsoft.Teams.Apps.DIConnect.Common.Services;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.Teams;

    /// <summary>
    /// Class handles methods to validate member.
    /// </summary>
    public class MemberValidationHelper : IMemberValidationHelper
    {
        /// <summary>
        /// Sends logs to the Application Insights service.
        /// </summary>
        private readonly ILogger<MemberValidationHelper> logger;

        /// <summary>
        /// Provider to fetch team details from bot adapter.
        /// </summary>
        private readonly ITeamMembersService memberService;

        /// <summary>
        /// Provider to fetch app settings details from storage.
        /// </summary>
        private readonly IAppSettingsService appSettingsService;

        /// <summary>
        /// Cache for storing authorization result.
        /// </summary>
        private readonly IMemoryCache memoryCache;

        /// <summary>
        /// Admin team Id.
        /// </summary>
        private readonly string adminTeamId;

        /// <summary>
        /// Tenant Id.
        /// </summary>
        private readonly string tenantId;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberValidationHelper"/> class.
        /// </summary>
        /// <param name="authenticationOptions">Represents a set of key/value bot settings.</param>
        /// <param name="memberService">Teams member service.</param>
        /// <param name="appSettingsService">App settings service.</param>
        /// <param name="memoryCache">MemoryCache instance for caching authorization result.</param>
        /// <param name="logger">Logger implementation to send logs to the logger service.</param>
        public MemberValidationHelper(
            IOptions<AuthenticationOptions> authenticationOptions,
            IAppSettingsService appSettingsService,
            ITeamMembersService memberService,
            IMemoryCache memoryCache,
            ILogger<MemberValidationHelper> logger)
        {
            authenticationOptions = authenticationOptions ?? throw new ArgumentNullException(nameof(authenticationOptions));
            this.memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
            this.appSettingsService = appSettingsService ?? throw new ArgumentNullException(nameof(appSettingsService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));

            this.adminTeamId = authenticationOptions.Value.AdminTeamId;
            this.tenantId = authenticationOptions.Value.AzureAdTenantId;
        }

        /// <summary>
        /// Check if a user is a member of a admin team.
        /// </summary>
        /// <param name="userAadObjectId">The user's Azure Active Directory object id.</param>
        /// <returns>The flag indicates that the user is a part of certain team or not.</returns>
        public async Task<bool> IsAdminTeamMemberAsync(string userAadObjectId)
        {
            try
            {
                bool isCacheEntryExists = this.memoryCache.TryGetValue(this.GetCacheKey(userAadObjectId), out bool isUserValidMember);
                if (!isCacheEntryExists)
                {
                    var serviceUrl = await this.appSettingsService.GetServiceUrlAsync();
                    if (serviceUrl == null)
                    {
                        this.logger.LogWarning($"Failed to service URL details for user {userAadObjectId}");

                        return false;
                    }

                    // Sync members.
                    var teamMembers = await this.memberService.GetMembersAsync(
                    teamId: this.adminTeamId,
                    tenantId: this.tenantId,
                    serviceUrl: serviceUrl);

                    if (teamMembers == null || !teamMembers.Where(row => row.AadId == userAadObjectId).Any())
                    {
                        isUserValidMember = false;
                    }
                    else
                    {
                        isUserValidMember = true;
                    }

                    this.memoryCache.Set(this.GetCacheKey(userAadObjectId), isUserValidMember, TimeSpan.FromMinutes(Constants.CacheDurationInMinutes));
                }

                return isUserValidMember;
            }
#pragma warning disable CA1031 // Catching general exceptions to log exception details in telemetry client.
            catch (Exception ex)
#pragma warning restore CA1031 // Catching general exceptions to log exception details in telemetry client.
            {
                this.logger.LogError(ex, $"Error occurred while fetching team member for team: {this.adminTeamId} - user object id: {userAadObjectId} ");

                // Return false if the member is not found in team id or either of the information is incorrect.
                // Caller should handle false value to throw unauthorized if required.
                return false;
            }
        }

        /// <summary>
        /// Get admin team cache key value.
        /// </summary>
        /// <param name="userAadObjectId">Unique id of Azure Active Directory of user.</param>
        /// <returns>Returns a admin team cache key value.</returns>
        private string GetCacheKey(string userAadObjectId)
        {
            return $"{this.adminTeamId}{userAadObjectId}{PolicyNames.AdminTeamMemberCacheKey}";
        }
    }
}
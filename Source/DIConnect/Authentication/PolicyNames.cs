// <copyright file="PolicyNames.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Authentication
{
    /// <summary>
    /// This class lists the names of the custom authorization policies in the project.
    /// </summary>
    public class PolicyNames
    {
        /// <summary>
        /// The name of the authorization policy, MSGraphGroupDataPolicy.
        /// </summary>
        public const string MSGraphGroupDataPolicy = "MSGraphGroupDataPolicy";

        /// <summary>
        /// The name of the authorization policy, MustBeAdminTeamMemberPolicy.
        /// Indicates that user is a member of admin team and has permission to access communication and configuration tab.
        /// </summary>
        public const string MustBeAdminTeamMemberPolicy = "MustBeAdminTeamMemberPolicy";

        /// <summary>
        /// The name of the authorization policy, MustBeTeamOwnerOrAdminUser.
        /// Indicates that user is a owner of team or member of admin team.
        /// </summary>
        public const string MustBeTeamOwnerOrAdminUserPolicy = "MustBeTeamOwnerOrAdminUser";

        /// <summary>
        /// The name of the authorization policy, MustBeTeamMemberPolicy.
        /// Indicates that user is a member of team.
        /// </summary>
        public const string MustBeTeamMemberPolicy = "MustBeTeamMemberPolicy";

        /// <summary>
        /// Cache key for admin team member.
        /// </summary>
        public const string AdminTeamMemberCacheKey = "_AM";

        /// <summary>
        /// Cache key for team owner.
        /// </summary>
        public const string TeamOwnerCacheKey = "_TO";

        /// <summary>
        /// Cache key for team Member.
        /// </summary>
        public const string TeamMemberCacheKey = "_TM";
    }
}
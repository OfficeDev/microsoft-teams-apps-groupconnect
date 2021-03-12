// <copyright file="MustBeAdminTeamMemberHandler.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Authentication
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Teams.Apps.DIConnect.Authentication.AuthenticationHelper;
    using Microsoft.Teams.Apps.DIConnect.Common;

    /// <summary>
    /// This class is an authorization handler, which handles the authorization requirement.
    /// </summary>
    public class MustBeAdminTeamMemberHandler : IAuthorizationHandler
    {
        /// <summary>
        /// Instance of MemberValidationService to validate member.
        /// </summary>
        private readonly IMemberValidationHelper memberValidationHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MustBeAdminTeamMemberHandler"/> class.
        /// </summary>
        /// <param name="memberValidationHelper">Instance of MemberValidationService to validate member.</param>
        public MustBeAdminTeamMemberHandler(IMemberValidationHelper memberValidationHelper)
        {
            this.memberValidationHelper = memberValidationHelper ?? throw new ArgumentNullException(nameof(memberValidationHelper));
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
                // Check if current sign-in user is the part of admin team.
                if (requirement is MustBeAdminTeamMemberRequirement
                    && await this.memberValidationHelper.IsAdminTeamMemberAsync(oidClaim.Value))
                {
                    context.Succeed(requirement);
                    break;
                }
            }
        }
    }
}
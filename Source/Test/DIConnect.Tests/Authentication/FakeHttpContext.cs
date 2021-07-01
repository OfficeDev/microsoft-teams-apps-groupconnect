// <copyright file="FakeHttpContext.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Tests.Authentication
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Security.Principal;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Teams.Apps.DIConnect.Authentication;
    using Moq;

    /// <summary>
    /// Class to fake HTTP Context
    /// </summary>
    public class FakeHttpContext
    {
        /// <summary>
        /// Make fake HTTP context for unit testing.
        /// </summary>
        /// <returns>Fake HTTP context.</returns>
        public static HttpContext GetMockHttpContextWithUserClaims()
        {
            var context = new Mock<HttpContext>();
            var request = new Mock<HttpRequest>();
            var response = new Mock<HttpContext>();
            var user = new Mock<ClaimsPrincipal>();
            var identity = new Mock<IIdentity>();
            var claim = new Claim[]
            {
                new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", AuthenticationTestData.userObjectId),
            };

            context.Setup(ctx => ctx.User).Returns(user.Object);

            user.Setup(ctx => ctx.Identity).Returns(identity.Object);
            user.Setup(ctx => ctx.Claims).Returns(claim);

            identity.Setup(id => id.IsAuthenticated).Returns(true);
            identity.Setup(id => id.Name).Returns("test");

            return context.Object;
        }

        /// <summary>
        /// Get default http context with user identity.
        /// </summary>
        /// <returns>http context</returns>
        public static HttpContext GetDefaultContextWithUserIdentity()
        {
            var context = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(
                        new ClaimsIdentity(
                            new Claim[]
                            {
                            new Claim(
                                "http://schemas.microsoft.com/identity/claims/objectidentifier",
                                AuthenticationTestData.userObjectId),
                            })),
            };

            context.Request.Headers["Authorization"] = "fake_token";

            return context;
        }

        /// <summary>
        /// Get authorization handler context.
        /// </summary>
        /// <returns>authorization handler context.</returns>
        public static AuthorizationHandlerContext GetAuthorizationHandlerContextForTeamOwner()
        {
            var requirement = new[] { new MustBeTeamOwnerOrAdminUserHandlerRequirement() };

            var context = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(
                        new ClaimsIdentity(
                            new Claim[]
                            {
                            new Claim(
                                "http://schemas.microsoft.com/identity/claims/objectidentifier",
                                AuthenticationTestData.userObjectId),
                            })),
            };

            context.Request.Headers["Authorization"] = "fake_token";
            context.Request.QueryString = new QueryString("?groupId=1a1cce71-2833-4345-86e2-e9047f73e6af");

            var filters = new List<IFilterMetadata>();

            var resource = new AuthorizationFilterContext(new ActionContext(context, new AspNetCore.Routing.RouteData(), new AspNetCore.Mvc.Abstractions.ActionDescriptor()), filters);

            return new AuthorizationHandlerContext(requirement, context.User, resource);
        }

        /// <summary>
        /// Get authorization handler context.
        /// </summary>
        /// <returns>authorization handler context.</returns>
        public static AuthorizationHandlerContext GetAuthorizationHandlerContextForTeamOwnerOrAdminUser()
        {
            var requirement = new[] { new MustBeTeamOwnerOrAdminUserHandlerRequirement() };

            var context = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(
                        new ClaimsIdentity(
                            new Claim[]
                            {
                            new Claim(
                                "http://schemas.microsoft.com/identity/claims/objectidentifier",
                                AuthenticationTestData.userObjectId),
                            })),
            };

            context.Request.Headers["Authorization"] = "fake_token";

            var filters = new List<IFilterMetadata>();

            var resource = new AuthorizationFilterContext(new ActionContext(context, new AspNetCore.Routing.RouteData(), new AspNetCore.Mvc.Abstractions.ActionDescriptor()), filters);

            return new AuthorizationHandlerContext(requirement, context.User, resource);
        }

        /// <summary>
        /// Get authorization handler context.
        /// </summary>
        /// <returns>authorization handler context.</returns>
        public static AuthorizationHandlerContext GetAuthorizationHandlerContextForAdminTeamMember()
        {
            var requirement = new[] { new MustBeAdminTeamMemberRequirement() };

            var context = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(
                        new ClaimsIdentity(
                            new Claim[]
                            {
                            new Claim(
                                "http://schemas.microsoft.com/identity/claims/objectidentifier",
                                AuthenticationTestData.userObjectId),
                            })),
            };

            context.Request.Headers["Authorization"] = "fake_token";

            var filters = new List<IFilterMetadata>();

            var resource = new AuthorizationFilterContext(new ActionContext(context, new AspNetCore.Routing.RouteData(), new AspNetCore.Mvc.Abstractions.ActionDescriptor()), filters);

            return new AuthorizationHandlerContext(requirement, context.User, resource);
        }

        /// <summary>
        /// Get authorization handler context.
        /// </summary>
        /// <returns>authorization handler context.</returns>
        public static AuthorizationHandlerContext GetAuthorizationHandlerContextForTeamMember()
        {
            var requirement = new[] { new MustBeTeamMemberRequirement() };

            var context = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(
                        new ClaimsIdentity(
                            new Claim[]
                            {
                            new Claim(
                                "http://schemas.microsoft.com/identity/claims/objectidentifier",
                                 AuthenticationTestData.userObjectId),
                            })),
            };

            context.Request.Headers["Authorization"] = "fake_token";
            context.Request.QueryString = new QueryString("?groupId=1a1cce71-2833-4345-86e2-e9047f73e6af");

            var filters = new List<IFilterMetadata>();

            var resource = new AuthorizationFilterContext(new ActionContext(context, new AspNetCore.Routing.RouteData(), new AspNetCore.Mvc.Abstractions.ActionDescriptor()), filters);

            return new AuthorizationHandlerContext(requirement, context.User, resource);
        }
    }
}
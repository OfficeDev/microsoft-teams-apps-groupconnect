// <copyright file="ParseTeamIdExtension.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Extensions
{
    using System;
    using System.Text.RegularExpressions;
    using System.Web;

    /// <summary>
    /// Parse team id extension.
    /// </summary>
    public static class ParseTeamIdExtension
    {
        /// <summary>
        /// Get Team's unique GUID id from Teams deep link URL.
        /// </summary>
        /// <param name="teamLink">Deep link to get the team id.</param>
        /// <returns>A team id from the deep link URL.</returns>
        public static string GetTeamIdFromDeepLink(string teamLink)
        {
            // Team id regex match
            // for a pattern like https://teams.microsoft.com/l/team/19%3a64c719819fb1412db8a28fd4a30b581a%40thread.tacv2/conversations?groupId=53b4782c-7c98-4449-993a-441870d10af9&tenantId=72f988bf-86f1-41af-91ab-2d7cd011db47
            // regex checks for 19%3a64c719819fb1412db8a28fd4a30b581a%40thread.tacv2
            var match = Regex.Match(teamLink, @"teams.microsoft.com/l/team/(\S+)/");
            if (!match.Success)
            {
                throw new ArgumentException("Invalid Team found.");
            }

            return HttpUtility.UrlDecode(match.Groups[1].Value);
        }

        /// <summary>
        /// Get Team's tenant id from Teams deep link URL.
        /// </summary>
        /// <param name="teamLink">Deep link to get the team id.</param>
        /// <returns>A tenant id from the deep link URL.</returns>
        public static string GetTenantIdFromDeepLink(string teamLink)
        {
            // Team id regex match
            // for a pattern like https://teams.microsoft.com/l/team/19%3a64c719819fb1412db8a28fd4a30b581a%40thread.tacv2/conversations?groupId=53b4782c-7c98-4449-993a-441870d10af9&tenantId=72f988bf-86f1-41af-91ab-2d7cd011db47
            // regex checks for 19%3a64c719819fb1412db8a28fd4a30b581a%40thread.tacv2
            var match = Regex.Match(teamLink, @"teams.microsoft.com/l/team/(\S+)/conversations(\S+)");
            if (!match.Success)
            {
                throw new ArgumentException("Invalid Team found.");
            }

            return HttpUtility.UrlDecode(match.Groups[2].Value.Split("tenantId=")[1]);
        }

        /// <summary>
        /// Get Team's group id from Teams deep link URL.
        /// </summary>
        /// <param name="teamLink">Deep link to get the team id.</param>
        /// <returns>A group id from the deep link URL.</returns>
        public static string GetGroupIdFromDeepLink(string teamLink)
        {
            // Team id regex match
            // for a pattern like https://teams.microsoft.com/l/team/19%3a64c719819fb1412db8a28fd4a30b581a%40thread.tacv2/conversations?groupId=53b4782c-7c98-4449-993a-441870d10af9&tenantId=72f988bf-86f1-41af-91ab-2d7cd011db47
            // regex checks for 19%3a64c719819fb1412db8a28fd4a30b581a%40thread.tacv2
            var match = Regex.Match(teamLink, @"teams.microsoft.com/l/team/(\S+)/conversations(\S+)");
            if (!match.Success)
            {
                throw new ArgumentException("Invalid Team found.");
            }

            return HttpUtility.UrlDecode(match.Groups[2].Value.Split("&")[0].Split("groupId=")[1]);
        }
    }
}
// <copyright file="Constants.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common
{
    /// <summary>
    /// Constants values used in across project.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Cache duration in minutes.
        /// </summary>
        public const int CacheDurationInMinutes = 60;

        /// <summary>
        /// get the group read all scope.
        /// </summary>
        public const string ScopeGroupReadAll = "Group.Read.All";

        /// <summary>
        /// AppCatalog Read All scope.
        /// </summary>
        public const string ScopeAppCatalogReadAll = "AppCatalog.Read.All";

        /// <summary>
        /// get the user read scope.
        /// </summary>
        public const string ScopeUserRead = "User.Read";

        /// <summary>
        /// scope claim type.
        /// </summary>
        public const string ClaimTypeScp = "scp";

        /// <summary>
        /// authorization scheme.
        /// </summary>
        public const string BearerAuthorizationScheme = "Bearer";

        /// <summary>
        /// claim type user id.
        /// </summary>
        public const string ClaimTypeUserId = "http://schemas.microsoft.com/identity/claims/objectidentifier";

        /// <summary>
        /// blob container name.
        /// </summary>
        public const string BlobContainerName = "exportdatablobs";

        /// <summary>
        /// get the group type Hidden Membership.
        /// </summary>
        public const string HiddenMembership = "HiddenMembership";

        /// <summary>
        /// get the header key for graph permission type.
        /// </summary>
        public const string PermissionTypeKey = "x-api-permission";

        /// <summary>
        /// get the default graph scope.
        /// </summary>
        public const string ScopeDefault = "https://graph.microsoft.com/.default";

        /// <summary>
        /// get the OData next page link.
        /// </summary>
        public const string ODataNextPageLink = "@odata.nextLink";

        /// <summary>
        /// Represent command text for approval notification card.
        /// </summary>
        public const string ApprovedText = "Approved";

        /// <summary>
        /// Represent command text for reject notification card.
        /// </summary>
        public const string RejectedText = "Rejected";

        /// <summary>
        /// Default partition key for employee resource group.
        /// </summary>
        public const string ResourceGroupTablePartitionKey = "Group";

        /// <summary>
        /// Represents employee resource group table name.
        /// </summary>
        public const string EmployeeResourceGroupEntityTableName = "EmployeeResourceGroup";

        /// <summary>
        /// Deep link to initiate chat.
        /// </summary>
        public const string ChatInitiateURL = "https://teams.microsoft.com/l/chat/0/0";

        /// <summary>
        /// Represents share command for bot.
        /// </summary>
        public const string ShareCommand = "share";

        /// <summary>
        /// Pause matches command for bot.
        /// </summary>
        public const string PauseMatchesCommand = "pause matches";

        /// <summary>
        /// Configure matches command for bot.
        /// </summary>
        public const string ConfigureMatchesCommand = "configure matches";

        /// <summary>
        /// Update matches command for bot.
        /// </summary>
        public const string UpdateMatchesCommand = "update matches";
    }
}
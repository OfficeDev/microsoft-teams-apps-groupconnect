// <copyright file="CardCacheConstants.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect
{
    /// <summary>
    /// Constants to list keys used by cache layers in application.
    /// </summary>
    public static class CardCacheConstants
    {
        /// <summary>
        /// Cache key for welcome card template.
        /// </summary>
        public const string WelcomeJsonTemplate = "_WCJTemplate";

        /// <summary>
        /// Cache key for configure user matches card template.
        /// </summary>
        public const string ConfigureMatchesJsonTemplate = "_CMJTemplate";

        /// <summary>
        /// Cache key for resume user matches card template.
        /// </summary>
        public const string ResumeMatchesJsonTemplate = "_RMJTemplate";

        /// <summary>
        /// Cache key for user pair-up teams mapping card template.
        /// </summary>
        public const string PairUpTeamsMappingJsonTemplate = "_PTMJTemplate";

        /// <summary>
        /// Cache key for share feedback card template.
        /// </summary>
        public const string ShareFeedbackJsonTemplate = "_SFBJTemplate";

        /// <summary>
        /// Cache key for QnA response card template.
        /// </summary>
        public const string QnAResponseJsonTemplate = "_QARJTemplate";

        /// <summary>
        /// Cache key for admin team approve/reject card template.
        /// </summary>
        public const string AdminApprovalCardJsonTemplate = "_AAJTemplate";

        /// <summary>
        /// Cache key for admin team notification card template.
        /// </summary>
        public const string AdminNotificationCardJsonTemplate = "_AANJTemplate";
    }
}
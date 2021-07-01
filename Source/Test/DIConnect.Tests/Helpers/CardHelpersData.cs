// <copyright file="CardHelpersData.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Tests.Helpers
{
    using Microsoft.Azure.CognitiveServices.Knowledge.QnAMaker.Models;
    using Microsoft.Bot.Schema.Teams;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.EmployeeResourceGroup;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.UserPairupMapping;
    using Microsoft.Teams.Apps.DIConnect.Models;
    using Microsoft.Teams.Apps.DIConnect.Models.CardSetting;
    using System.Collections.Generic;

    /// <summary>
    /// Class that contains test data for card helper methods.
    /// </summary>
    public static class CardHelpersData
    {
        /// <summary>
        /// Personal scope welcome card file path.
        /// </summary>
        public static readonly string PersonalScopeWelcomeCardFilePath = ".\\Helpers\\Cards\\WelcomeCardPersonalScope_TestResult.json";

        /// <summary>
        /// Feedback notification card file path.
        /// </summary>
        public static readonly string FeedbackNotificationCardFilePath = ".\\Helpers\\Cards\\FeedbackNotificationCard_TestResult.json";

        /// <summary>
        /// Resume pair up matches card file path.
        /// </summary>
        public static readonly string ResumePairUpMatchesCardFilePath = ".\\Helpers\\Cards\\ResumePairUpMatchesCard_TestResult.json";

        /// <summary>
        /// Configure matches card file path.
        /// </summary>
        public static readonly string ConfigureMatchesCardFilePath = ".\\Helpers\\Cards\\ConfigureMatchesCard_TestResult.json";

        /// <summary>
        /// Approval card file path.
        /// </summary>
        public static readonly string ApprovalCardFilePath = ".\\Helpers\\Cards\\ApprovalCard_TestResult.json";

        /// <summary>
        /// Approval updated card file path.
        /// </summary>
        public static readonly string ApprovalUpdatedCardFilePath = ".\\Helpers\\Cards\\ApprovalUpdatedCard_TestResult.json";

        /// <summary>
        /// User pair up matches file path.
        /// </summary>
        public static readonly string UserPairUpMatchesCardFilePath = ".\\Helpers\\Cards\\UserPairUpMatchesCard_TestResult.json";

        /// <summary>
        /// QnA with prompts response card file path.
        /// </summary>
        public static readonly string QnAWithPromptsResponseCardFilePath = ".\\Helpers\\Cards\\QnAWithPromptsResponseCard_TestResult.json";

        /// <summary>
        /// QnA response card file path.
        /// </summary>
        public static readonly string QnAResponseCardFilePath = ".\\Helpers\\Cards\\QnAResponseCard_TestResult.json";

        /// <summary>
        /// Represents submit action data for teams behavior.
        /// </summary>
        public static readonly SubmitActionDataForTeamsBehavior submitActionData = new SubmitActionDataForTeamsBehavior()
        {
            FeedbackType = "Helpful",
            FeedbackDescription = "Testing the description",
        };

        /// <summary>
        /// Represents team channel account data.
        /// </summary>
        public static readonly TeamsChannelAccount teamsChannelAccount = new TeamsChannelAccount()
        {
            GivenName = "Mod",
            UserPrincipalName = "@microsoft.com"
        };

        /// <summary>
        /// Represents employee resource group entity data.
        /// </summary>
        public static readonly EmployeeResourceGroupEntity groupEntity = new EmployeeResourceGroupEntity()
        {
           GroupId = "123",
           GroupName = "Resource group name",
           GroupDescription = "Resource group description",
           ApprovalStatus = 1,
           IncludeInSearchResults = true,
           Location = "West US"
        };

        /// <summary>
        /// Represents list of team pair up data.
        /// </summary>
        public static readonly IEnumerable<TeamPairUpData> teamPairUpDatas = new List<TeamPairUpData>()
        {
            new TeamPairUpData()
            {
                TeamDisplayName = "Team name 1",
                TeamId = "12"
            },
            new TeamPairUpData() 
            { 
                TeamDisplayName = "Team name 2",
                TeamId = "34" 
            },
        };

        /// <summary>
        /// Represents list of team pair up mapping entities data.
        /// </summary>
        public static readonly IEnumerable<TeamUserPairUpMappingEntity> teamUserPairUpMappingEntities = new List<TeamUserPairUpMappingEntity>()
        {
            new TeamUserPairUpMappingEntity()
            {
                IsPaused = true,
                TeamId = "12"
            },
            new TeamUserPairUpMappingEntity()
            {
                IsPaused = true,
                TeamId = "34"
            },
        };

        /// <summary>
        /// Represents empty list of team pair up mapping entities data.
        /// </summary>
        public static readonly IEnumerable<TeamUserPairUpMappingEntity> emptyEntities = new List<TeamUserPairUpMappingEntity>() { };

        /// <summary>
        /// Represents list of prompts data.
        /// </summary>
        public static readonly IList<PromptDTO> prompts = new List<PromptDTO>()
        {
           new PromptDTO()
           {
               DisplayText = "Prompt question 1"
           },
           new PromptDTO()
           { 
               DisplayText = "Prompt question 2"
           },
        };

        /// <summary>
        /// Represents empty list of prompts data.
        /// </summary>
        public static readonly IList<PromptDTO> emptyPrompts = new List<PromptDTO>() { };
        
    }
}
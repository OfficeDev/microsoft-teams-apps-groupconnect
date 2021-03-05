// <copyright file="UserTeamMappingsHelper.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Localization;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.EmployeeResourceGroup;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.UserPairupMapping;
    using Microsoft.Teams.Apps.DIConnect.Common.Resources;
    using Microsoft.Teams.Apps.DIConnect.Models.CardSetting;

    /// <summary>
    /// Helper class for user mappings for a Team.
    /// </summary>
    public class UserTeamMappingsHelper
    {
        /// <summary>
        /// Repository for employee resource group.
        /// </summary>
        private readonly EmployeeResourceGroupRepository employeeResourceGroupRepository;

        /// <summary>
        /// Repository for team pair matching.
        /// </summary>
        private readonly TeamUserPairUpMappingRepository teamUserPairupMappingRepository;

        /// <summary>
        /// Instance of class that handles user pair-up matches card helper methods.
        /// </summary>
        private readonly CardHelper cardHelper;

        /// <summary>
        /// The current culture's string localizer.
        /// </summary>
        private readonly IStringLocalizer<Strings> localizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserTeamMappingsHelper"/> class.
        /// </summary>
        /// <param name="teamUserPairupMappingRepository">Instance of team pair-up repository to access user pair-up matches.</param>
        /// <param name="employeeResourceGroupRepository">Instance of employee resource group repository to access resource group details.</param>
        /// <param name="cardHelper">Instance of class that handles user pair-up matches card helper methods.</param>
        /// <param name="localizer">The current culture's string localizer.</param>
        public UserTeamMappingsHelper(
            TeamUserPairUpMappingRepository teamUserPairupMappingRepository,
            EmployeeResourceGroupRepository employeeResourceGroupRepository,
            CardHelper cardHelper,
            IStringLocalizer<Strings> localizer)
        {
            this.teamUserPairupMappingRepository = teamUserPairupMappingRepository ?? throw new ArgumentNullException(nameof(teamUserPairupMappingRepository));
            this.employeeResourceGroupRepository = employeeResourceGroupRepository ?? throw new ArgumentNullException(nameof(employeeResourceGroupRepository));
            this.cardHelper = cardHelper ?? throw new ArgumentNullException(nameof(cardHelper));
            this.localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        /// <summary>
        /// To send user team mappings card.
        /// </summary>
        /// <param name="turnContext">Context object containing information cached for a single turn of conversation with a user.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>A task representing asynchronous operation.</returns>
        public async Task SendUserTeamMappingsCardAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var userId = turnContext.Activity.From.AadObjectId;
            var resourceGroupDetails = await this.employeeResourceGroupRepository.GetResourceGroupsByTypeAsync((int)ResourceGroupType.Teams);
            var userTeamMappingEntities = await this.teamUserPairupMappingRepository.GetAllAsync(userId);
            var teamMappingsForRecipient = new List<TeamPairUpData>();

            if (resourceGroupDetails != null && resourceGroupDetails.Any() && userTeamMappingEntities != null && userTeamMappingEntities.Any())
            {
                foreach (var userTeamEntity in userTeamMappingEntities)
                {
                    var resourceGroupEntity = resourceGroupDetails.FirstOrDefault(row => row.TeamId == userTeamEntity.TeamId);
                    if (resourceGroupEntity != null)
                    {
                        teamMappingsForRecipient.Add(new TeamPairUpData
                        {
                            TeamDisplayName = resourceGroupEntity.GroupName,
                            TeamId = userTeamEntity.TeamId,
                        });
                    }
                }

                var configureUserMatchesCard = MessageFactory.Attachment(this.cardHelper.GetUserPairUpMatchesCard(teamMappingsForRecipient, userTeamMappingEntities));
                await turnContext.SendActivityAsync(configureUserMatchesCard, cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(this.localizer.GetString("NoMatchesToManageText")));
            }
        }
    }
}
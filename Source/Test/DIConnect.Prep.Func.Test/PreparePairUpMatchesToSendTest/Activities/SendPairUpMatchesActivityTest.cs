// <copyright file="SendPairUpMatchesActivityTest.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Prep.Func.Test.PreparePairUpMatchesToSend.Activities
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.MessageQueues.UserPairupQueue;
    using Microsoft.Teams.Apps.DIConnect.Prep.Func.PreparePairUpMatchesToSend.Activities;
    using Moq;
    using Xunit;

    /// <summary>
    /// SendPairUpMatchesActivity test class.
    /// </summary>
    public class SendPairUpMatchesActivityTest
    {
        private readonly Mock<IUserPairUpQueue> userPairUpQueue = new Mock<IUserPairUpQueue>();

        /// <summary>
        /// Sends pair up matches to user pair-up queue.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task SendPairUpMatchesActivitySuccessTest()
        {
            // Arrange
            var sendPairUpMatchesActivity = this.SendPairUpMatchesActivity();
            string teamId = "00000000-0000-0000-0000-000000000000";
            Mock<ILogger> logger = new Mock<ILogger>();

            UserData userData = new UserData()
            {
                UserGivenName = "fdsz",
                UserObjectId = "231231",
                UserPrincipalName = "kdashs",
            };

            UserPairUpQueueMessageContent userPairUpQueueMessageContent = new UserPairUpQueueMessageContent()
            {
                TeamId = "00000000-0000-0000-0000-000000000000",
                TeamName = "dgavskjda",
            };

            List<TeamUserMapping> teamUserMappings = new List<TeamUserMapping>()
            {
                new TeamUserMapping { TeamId = "00000000-0000-0000-0000-000000000000", TeamName = "abc" },
                new TeamUserMapping { TeamId = "00000000-0000-0000-0000-000000000000", TeamName = "abc" },
                new TeamUserMapping { TeamId = "00000000-0000-0000-0000-000000000000", TeamName = "abc" },
            };

            this.userPairUpQueue
                .Setup(x => x.SendAsync(It.IsAny<IEnumerable<UserPairUpQueueMessageContent>>()))
                .Returns(Task.CompletedTask);

            // Act
            Func<Task> task = async () => await sendPairUpMatchesActivity.RunAsync((teamId, teamUserMappings), logger.Object);

            // Assert
            await task.Should().NotThrowAsync();
            this.userPairUpQueue.Verify(x => x.SendAsync(It.IsAny<IEnumerable<UserPairUpQueueMessageContent>>()), Times.AtLeastOnce());
        }

        /// <summary>
        /// Prepare pair-up matches.
        /// </summary>
        [Fact]
        public void PrepareMatchesTestCase()
        {
            // Arrange
            var sendPairUpMatchesActivity = this.SendPairUpMatchesActivity();
            Mock<ILogger> logger = new Mock<ILogger>();

            List<Tuple<TeamUserMapping, TeamUserMapping>> tupleteamUserMappings = new List<Tuple<TeamUserMapping, TeamUserMapping>>();

            tupleteamUserMappings.Add(new Tuple<TeamUserMapping, TeamUserMapping>(
                new TeamUserMapping
                { TeamId = "00000000-0000-0000-0000-000000000000", TeamName = "abc" },
                new TeamUserMapping { TeamId = "00000000-0000-0000-0000-000000000000", TeamName = "abc" }));

            List<TeamUserMapping> teamUserMappings = new List<TeamUserMapping>()
            {
                new TeamUserMapping { TeamId = "00000000-0000-0000-0000-000000000000", TeamName = "abc" },
                new TeamUserMapping { TeamId = "00000000-0000-0000-0000-000000000000", TeamName = "abc" },
                new TeamUserMapping { TeamId = "00000000-0000-0000-0000-000000000000", TeamName = "abc" },
            };

            // Act
            var task = sendPairUpMatchesActivity.PrepareMatches(teamUserMappings, logger.Object);

            // Assert
            task.Should().HaveCount(1);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetActivePaiSendPairUpMatchesActivityrUpUsersActivity"/> class.
        /// </summary>
        /// <returns>return the instance of GetActivePairUpUsersActivity.</returns>
        private SendPairUpMatchesActivity SendPairUpMatchesActivity()
        {
            return new SendPairUpMatchesActivity(this.userPairUpQueue.Object);
        }
    }
}
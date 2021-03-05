// <copyright file="SendPairUpMatchesActivity.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Prep.Func.PreparePairUpMatchesToSend.Activities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.MessageQueues.UserPairupQueue;
    using Microsoft.Teams.Apps.DIConnect.Prep.Func.PreparingToSend;

    /// <summary>
    /// Sends pair up matches to user pair-up queue.
    /// </summary>
    public class SendPairUpMatchesActivity
    {
        /// <summary>
        /// User pair-up queue service.
        /// </summary>
        private readonly UserPairUpQueue userPairUpQueue;

        /// <summary>
        /// The maximum number of messages that can be in one batch request to the service bus queue.
        /// </summary>
        private readonly int maxNumberOfMessagesInBatchRequest = 100;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendPairUpMatchesActivity"/> class.
        /// </summary>
        /// <param name="userPairUpQueue">User pair up queue service.</param>
        public SendPairUpMatchesActivity(
            UserPairUpQueue userPairUpQueue)
        {
            this.userPairUpQueue = userPairUpQueue ?? throw new ArgumentNullException(nameof(userPairUpQueue));
        }

        /// <summary>
        /// Run the activity.
        /// Sends pair up matches to user pair-up queue.
        /// </summary>
        /// <param name="input">Input.</param>
        /// <param name="log">The logger.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [FunctionName(FunctionNames.SendPairUpMatchesActivity)]
        public async Task RunAsync(
            [ActivityTrigger](string teamId, List<TeamUserMapping> teamUserMappings) input,
            ILogger log)
        {
            try
            {
                var userPairUpMatches = this.PrepareMatches(input.teamUserMappings, log);
                var messageBatch = userPairUpMatches.Select(
                recipient =>
                {
                    try
                    {
                        return new UserPairUpQueueMessageContent()
                        {
                            // Assigning a unique GUID value to each pair-up notification.
                            PairUpNotificationId = Guid.NewGuid().ToString(),
                            TeamId = input.teamId,
                            TeamName = recipient.Item1.TeamName,
                            PairUpUserData = new UserPairsMessage()
                            {
                                Recipient1 = new UserData()
                                {
                                    UserGivenName = recipient.Item1?.UserGivenName,
                                    UserPrincipalName = recipient.Item1?.UserPrincipalName,
                                    UserObjectId = recipient.Item1.UserObjectId,
                                },
                                Recipient2 = new UserData()
                                {
                                    UserGivenName = recipient.Item2?.UserGivenName,
                                    UserPrincipalName = recipient.Item2?.UserPrincipalName,
                                    UserObjectId = recipient.Item2.UserObjectId,
                                },
                            },
                        };
                    }
                    catch (Exception ex)
                    {
                        log.LogError($"Unable to prepare pair-up matches: {ex.Message} for Team: {recipient.Item1.TeamId}");
                        return null;
                    }
                });

                log.LogInformation($"Send user pair-up matches to queue");
                var batchCount = (int)Math.Ceiling((double)messageBatch.Count() / this.maxNumberOfMessagesInBatchRequest);
                for (int batchIndex = 0; batchIndex < batchCount; batchIndex++)
                {
                    var batchWisePairUpMatches = messageBatch
                    .Skip(batchIndex * this.maxNumberOfMessagesInBatchRequest)
                    .Take(this.maxNumberOfMessagesInBatchRequest);

                    await this.userPairUpQueue.SendAsync(batchWisePairUpMatches.Where(row => row != null));
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Unable to send user pair-up matches to queue: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Prepare randomized pair-up matches.
        /// </summary>
        /// <param name="teamPairUpMatches">List of user pair-up mapping entity.</param>
        /// <param name="log">The logger.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public List<Tuple<TeamUserMapping, TeamUserMapping>> PrepareMatches(List<TeamUserMapping> teamPairUpMatches, ILogger log)
        {
            var pairs = new List<Tuple<TeamUserMapping, TeamUserMapping>>();
            this.Randomize(teamPairUpMatches);

            if (teamPairUpMatches.Count > 0)
            {
                for (int i = 0; i < teamPairUpMatches.Count - 1; i += 2)
                {
                    pairs.Add(new Tuple<TeamUserMapping, TeamUserMapping>(teamPairUpMatches[i], teamPairUpMatches[i + 1]));
                }
            }

            log.LogInformation($"Prepared matches to send notification message : {pairs.Count()}");

            return pairs;
        }

        /// <summary>
        /// Randomize list of users.
        /// </summary>
        /// <typeparam name="T">Generic item type.</typeparam>
        /// <param name="items">List of users to randomize.</param>
        private void Randomize<T>(IList<T> items)
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());

            // For each spot in the array, pick
            // a random item to swap into that spot.
            for (int i = 0; i < items.Count - 1; i++)
            {
                int j = rand.Next(i, items.Count);
                T temp = items[i];
                items[i] = items[j];
                items[j] = temp;
            }
        }
    }
}
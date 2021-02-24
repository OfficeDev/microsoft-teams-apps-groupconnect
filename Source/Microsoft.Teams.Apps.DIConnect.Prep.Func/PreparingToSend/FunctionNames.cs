// <copyright file="FunctionNames.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Prep.Func.PreparingToSend
{
    /// <summary>
    /// Defines constants for function names.
    /// </summary>
    public static class FunctionNames
    {
        /// <summary>
        /// Prepare to send function.
        /// </summary>
        public const string PrepareToSendFunction = nameof(PrepareToSendFunction);

        /// <summary>
        /// Pair up function.
        /// </summary>
        public const string PairUpFunction = nameof(PairUpFunction);

        /// <summary>
        /// Prepare to send orchestrator function.
        /// </summary>
        public const string PrepareToSendOrchestrator = nameof(PrepareToSendOrchestrator);

        /// <summary>
        /// Sync recipients orchestrator function.
        /// </summary>
        public const string SyncRecipientsOrchestrator = nameof(SyncRecipientsOrchestrator);

        /// <summary>
        /// Prepare batches to send orchestrator function.
        /// </summary>
        public const string PrepareBatchesToSendOrchestrator = nameof(PrepareBatchesToSendOrchestrator);

        /// <summary>
        /// Teams conversation orchestrator.
        /// </summary>
        public const string TeamsConversationOrchestrator = nameof(TeamsConversationOrchestrator);

        /// <summary>
        /// Send queue orchestrator function.
        /// </summary>
        public const string SendQueueOrchestrator = nameof(SendQueueOrchestrator);

        /// <summary>
        /// Sync recipients and send batches to queue orchestrator function.
        /// </summary>
        public const string SyncRecipientsAndSendBatchesToQueueOrchestrator = nameof(SyncRecipientsAndSendBatchesToQueueOrchestrator);

        /// <summary>
        /// Process and store message activity function.
        /// </summary>
        public const string StoreMessageActivity = nameof(StoreMessageActivity);

        /// <summary>
        /// Sync all users activity function.
        /// </summary>
        public const string SyncAllUsersActivity = nameof(SyncAllUsersActivity);

        /// <summary>
        /// Sync Team members activity function.
        /// </summary>
        public const string SyncTeamMembersActivity = nameof(SyncTeamMembersActivity);

        /// <summary>
        /// Sync pair up members activity function.
        /// </summary>
        public const string SyncPairUpMembersActivity = nameof(SyncPairUpMembersActivity);

        /// <summary>
        /// Sync group members activity function.
        /// </summary>
        public const string SyncGroupMembersActivity = nameof(SyncGroupMembersActivity);

        /// <summary>
        /// Sync Teams activity function.
        /// </summary>
        public const string SyncTeamsActivity = nameof(SyncTeamsActivity);

        /// <summary>
        /// Get recipients activity function.
        /// </summary>
        public const string GetRecipientsActivity = nameof(GetRecipientsActivity);

        /// <summary>
        /// Get resource group entities activity function.
        /// </summary>
        public const string GetResourceGroupEntitiesActivity = nameof(GetResourceGroupEntitiesActivity);

        /// <summary>
        /// Get active pair up users activity function.
        /// </summary>
        public const string GetActivePairUpUsersActivity = nameof(GetActivePairUpUsersActivity);

        /// <summary>
        /// Get pending recipients (ie recipients with no conversation id in the database) activity function.
        /// </summary>
        public const string GetPendingRecipientsActivity = nameof(GetPendingRecipientsActivity);

        /// <summary>
        /// Teams conversation activity function.
        /// </summary>
        public const string TeamsConversationActivity = nameof(TeamsConversationActivity);

        /// <summary>
        /// Data aggregation activity function.
        /// </summary>
        public const string DataAggregationTriggerActivity = nameof(DataAggregationTriggerActivity);

        /// <summary>
        /// Update notification activity function.
        /// </summary>
        public const string UpdateNotificationStatusActivity = nameof(UpdateNotificationStatusActivity);

        /// <summary>
        /// Send batch messages to send queue activity function.
        /// </summary>
        public const string SendBatchMessagesActivity = nameof(SendBatchMessagesActivity);

        /// <summary>
        /// Send matches to user pair up queue activity function.
        /// </summary>
        public const string SendPairUpMatchesActivity = nameof(SendPairUpMatchesActivity);

        /// <summary>
        /// Handle failure activity function.
        /// </summary>
        public const string HandleFailureActivity = nameof(HandleFailureActivity);
    }
}
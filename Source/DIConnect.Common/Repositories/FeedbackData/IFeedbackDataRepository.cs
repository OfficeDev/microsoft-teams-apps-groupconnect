// <copyright file="IFeedbackDataRepository.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Repositories.FeedbackData
{
    using Microsoft.Teams.Apps.DIConnect.DIConnect.Common.FeedbackData;

    /// <summary>
    /// Interface for feedback data repository.
    /// </summary>
    public interface IFeedbackDataRepository : IRepository<FeedbackEntity>
    {
    }
}
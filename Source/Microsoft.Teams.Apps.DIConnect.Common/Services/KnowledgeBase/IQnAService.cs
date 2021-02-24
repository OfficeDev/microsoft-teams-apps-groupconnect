// <copyright file="IQnAService.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Common.Services
{
    using System.Threading.Tasks;
    using Microsoft.Azure.CognitiveServices.Knowledge.QnAMaker.Models;

    /// <summary>
    /// QnA maker service provider interface.
    /// </summary>
    public interface IQnAService
    {
        /// <summary>
        /// Get answer from knowledge base for a given question.
        /// </summary>
        /// <param name="question">Question text.</param>
        /// <returns>QnA search result object as response.</returns>
        Task<QnASearchResultList> GenerateAnswerAsync(string question);
    }
}
// <copyright file="CardHelperTest.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Tests.Helpers
{
    using System;
    using System.IO;
    using AdaptiveCards;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.DIConnect.Common.Resources;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.CommonBot;
    using Microsoft.Teams.Apps.DIConnect.Helpers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// Class that contains test methods for card helper.
    /// </summary>
    [TestClass]
    public class CardHelperTest
    {
        private Mock<ILogger<CardHelper>> logger;
        private IOptions<BotOptions> botOptions;
        private Mock<IStringLocalizer<Strings>> localizer;
        private Mock<IMemoryCache> memoryCache;
        private Mock<IWebHostEnvironment> hostingEnvironment;
        private CardHelper cardHelper;

        private readonly string groupCreatorName = "Mod";
        private readonly string approvalStatusText = "Approved";
        private readonly string questionText = "Question";
        private readonly string answerText = "Answer";
        private readonly string appbaseUri = "https://AppbaseUri";

        /// <summary>
        /// Initialize all test variables.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.logger = new Mock<ILogger<CardHelper>>();
            this.memoryCache = new Mock<IMemoryCache>();
            this.localizer = new Mock<IStringLocalizer<Strings>>();
            this.hostingEnvironment = new Mock<IWebHostEnvironment>();
            this.botOptions = Options.Create(new BotOptions { ManifestId = "123" });

            this.cardHelper = new CardHelper(
                this.logger.Object,
                this.memoryCache.Object,
                this.hostingEnvironment.Object,
                this.localizer.Object,
                this.botOptions);
        }

        /// <summary>
        /// Test case to check if personal scope welcome card is not null and have valid contents.
        /// </summary>
        [TestMethod]
        public void PersonalScopeWelcomeCardNotNullValidContent()
        {
            // ARRANGE
            this.hostingEnvironment
                .Setup(m => m.ContentRootPath)
                .Returns(".");

            this.memoryCache
                .Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns(Mock.Of<ICacheEntry>);

            var cardTemplate = File.ReadAllText(CardHelpersData.PersonalScopeWelcomeCardFilePath);
            var expectedCardJson = Newtonsoft.Json.JsonConvert.SerializeObject(AdaptiveCard.FromJson(cardTemplate).Card);

            // ACT
            var actualAttachmentCardResult = this.cardHelper.GetWelcomeNotificationCard();
            var actualCardJson = Newtonsoft.Json.JsonConvert.SerializeObject(actualAttachmentCardResult.Content);

            // ASSERT
            Assert.IsNotNull(actualAttachmentCardResult);
            Assert.AreEqual(expectedCardJson, actualCardJson);
        }

        /// <summary>
        /// Test case to check if QnA response card is not null and have valid contents.
        /// </summary>
        [TestMethod]
        public void QnAResponseCardNotNullValidContent()
        {
            // ARRANGE
            this.hostingEnvironment
                .Setup(m => m.ContentRootPath)
                .Returns(".");

            this.memoryCache
                .Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns(Mock.Of<ICacheEntry>);

            var cardTemplate = File.ReadAllText(CardHelpersData.QnAResponseCardFilePath);
            var expectedCardJson = Newtonsoft.Json.JsonConvert.SerializeObject(AdaptiveCard.FromJson(cardTemplate).Card);

            // ACT
            var actualAttachmentCardResult = this.cardHelper.GetQnAResponseNotificationCard(questionText, answerText, CardHelpersData.emptyPrompts, appbaseUri);
            var actualCardJson = Newtonsoft.Json.JsonConvert.SerializeObject(actualAttachmentCardResult.Content);

            // ASSERT
            Assert.IsNotNull(actualAttachmentCardResult);
            Assert.AreEqual(expectedCardJson, actualCardJson);
        }

        /// <summary>
        /// Test case to check if QnA with prompts response card is not null and have valid contents.
        /// </summary>
        [TestMethod]
        public void QnAWithPromptsResponseCardNotNullValidContent()
        {
            // ARRANGE
            this.hostingEnvironment
                .Setup(m => m.ContentRootPath)
                .Returns(".");

            this.memoryCache
                .Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns(Mock.Of<ICacheEntry>);

            var cardTemplate = File.ReadAllText(CardHelpersData.QnAWithPromptsResponseCardFilePath);
            var expectedCardJson = Newtonsoft.Json.JsonConvert.SerializeObject(AdaptiveCard.FromJson(cardTemplate).Card);

            // ACT
            var actualAttachmentCardResult = this.cardHelper.GetQnAResponseNotificationCard(questionText, answerText, CardHelpersData.prompts, appbaseUri);
            var actualCardJson = Newtonsoft.Json.JsonConvert.SerializeObject(actualAttachmentCardResult.Content);

            // ASSERT
            Assert.IsNotNull(actualAttachmentCardResult);
            Assert.AreEqual(expectedCardJson, actualCardJson);
        }

        /// <summary>
        /// Test case to check if approval card is not null and have valid contents.
        /// </summary>
        [TestMethod]
        public void ApprovalCardNotNullValidContent()
        {
            // ARRANGE
            this.hostingEnvironment
                .Setup(m => m.ContentRootPath)
                .Returns(".");

            this.memoryCache
                .Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns(Mock.Of<ICacheEntry>);

            var cardTemplate = File.ReadAllText(CardHelpersData.ApprovalCardFilePath);
            var expectedCardJson = Newtonsoft.Json.JsonConvert.SerializeObject(AdaptiveCard.FromJson(cardTemplate).Card);

            // ACT
            var actualAttachmentCardResult = this.cardHelper.GetApprovalCard(CardHelpersData.groupEntity, groupCreatorName);
            var actualCardJson = Newtonsoft.Json.JsonConvert.SerializeObject(actualAttachmentCardResult.Content);

            // ASSERT
            Assert.IsNotNull(actualAttachmentCardResult);
            Assert.AreEqual(expectedCardJson, actualCardJson);
        }

        /// <summary>
        /// Test case to check if approval updated card is not null and have valid contents.
        /// </summary>
        [TestMethod]
        public void ApprovalUpdatedCardNotNullValidContent()
        {
            // ARRANGE
            this.hostingEnvironment
                .Setup(m => m.ContentRootPath)
                .Returns(".");

            this.memoryCache
                .Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns(Mock.Of<ICacheEntry>);

            var cardTemplate = File.ReadAllText(CardHelpersData.ApprovalUpdatedCardFilePath);
            var expectedCardJson = Newtonsoft.Json.JsonConvert.SerializeObject(AdaptiveCard.FromJson(cardTemplate).Card);

            // ACT
            var actualAttachmentCardResult = this.cardHelper.GetApprovalCard(CardHelpersData.groupEntity, groupCreatorName, approvalStatusText);
            var actualCardJson = Newtonsoft.Json.JsonConvert.SerializeObject(actualAttachmentCardResult.Content);

            // ASSERT
            Assert.IsNotNull(actualAttachmentCardResult);
            Assert.AreEqual(expectedCardJson, actualCardJson);
        }

        /// <summary>
        /// Test case to check if feedback notification card is not null and have valid contents.
        /// </summary>
        [TestMethod]
        public void FeedbackNotificationCardNotNullValidContent()
        {
            // ARRANGE
            this.hostingEnvironment
                .Setup(m => m.ContentRootPath)
                .Returns(".");

            this.memoryCache
                .Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns(Mock.Of<ICacheEntry>);

            var cardTemplate = File.ReadAllText(CardHelpersData.FeedbackNotificationCardFilePath);
            var expectedCardJson = Newtonsoft.Json.JsonConvert.SerializeObject(AdaptiveCard.FromJson(cardTemplate).Card);

            // ACT
            var actualAttachmentCardResult = this.cardHelper.GetShareFeedbackNotificationCard(CardHelpersData.submitActionData, CardHelpersData.teamsChannelAccount);
            var actualCardJson = Newtonsoft.Json.JsonConvert.SerializeObject(actualAttachmentCardResult.Content);

            // ASSERT
            Assert.IsNotNull(actualAttachmentCardResult);
            Assert.AreNotEqual(expectedCardJson, actualCardJson);
        }

        /// <summary>
        /// Test case to check if configure matches card is not null and have valid contents.
        /// </summary>
        [TestMethod]
        public void ConfigureMatchesCardNotNullValidContent()
        {
            // ARRANGE
            this.hostingEnvironment
                .Setup(m => m.ContentRootPath)
                .Returns(".");

            this.memoryCache
                .Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns(Mock.Of<ICacheEntry>);

            var cardTemplate = File.ReadAllText(CardHelpersData.ConfigureMatchesCardFilePath);
            var expectedCardJson = Newtonsoft.Json.JsonConvert.SerializeObject(AdaptiveCard.FromJson(cardTemplate).Card);

            // ACT
            var actualAttachmentCardResult = this.cardHelper.GetConfigureMatchesNotificationCard();
            var actualCardJson = Newtonsoft.Json.JsonConvert.SerializeObject(actualAttachmentCardResult.Content);

            // ASSERT
            Assert.IsNotNull(actualAttachmentCardResult);
            Assert.AreEqual(expectedCardJson, actualCardJson);
        }

        /// <summary>
        /// Test case to check if user pair up matches card is not null and have valid contents.
        /// </summary>
        [TestMethod]
        public void UserPairUpMatchesNotNullValidContent()
        {
            // ARRANGE
            this.hostingEnvironment
                .Setup(m => m.ContentRootPath)
                .Returns(".");

            this.memoryCache
                .Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns(Mock.Of<ICacheEntry>);

            var cardTemplate = File.ReadAllText(CardHelpersData.UserPairUpMatchesCardFilePath);
            var expectedCardJson = Newtonsoft.Json.JsonConvert.SerializeObject(AdaptiveCard.FromJson(cardTemplate).Card);

            // ACT
            var actualAttachmentCardResult = this.cardHelper.GetUserPairUpMatchesCard(CardHelpersData.teamPairUpDatas, CardHelpersData.teamUserPairUpMappingEntities);
            var actualCardJson = Newtonsoft.Json.JsonConvert.SerializeObject(actualAttachmentCardResult.Content);

            // ASSERT
            Assert.IsNotNull(actualAttachmentCardResult);
            Assert.AreEqual(expectedCardJson, actualCardJson);
        }

        /// <summary>
        /// Test case to throw exception while passing null data to user pair up matches card.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UserPairUpMatchesArgumentNullException()
        {
            // ACT
            this.cardHelper.GetUserPairUpMatchesCard(null, null);
        }

        /// <summary>
        /// Test case to return null while passing empty data to user pair up matches card.
        /// </summary>
        [TestMethod]
        public void UserPairUpMatchesReturnsNull()
        {
            // ACT
            var actualAttachmentCardResult =  this.cardHelper.GetUserPairUpMatchesCard(CardHelpersData.teamPairUpDatas, CardHelpersData.emptyEntities);

            // ASSERT
            Assert.IsNull(actualAttachmentCardResult);
        }

        /// <summary>
        /// Test case to check if resume pair up matches is not null and have valid contents.
        /// </summary>
        [TestMethod]
        public void ResumePairUpMatchesCardNotNullValidContent()
        {
            // ARRANGE
            this.hostingEnvironment
                .Setup(m => m.ContentRootPath)
                .Returns(".");

            this.memoryCache
                .Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns(Mock.Of<ICacheEntry>);

            var cardTemplate = File.ReadAllText(CardHelpersData.ResumePairUpMatchesCardFilePath);
            var expectedCardJson = Newtonsoft.Json.JsonConvert.SerializeObject(AdaptiveCard.FromJson(cardTemplate).Card);

            // ACT
            var actualAttachmentCardResult = this.cardHelper.GetResumePairupNotificationCard();
            var actualCardJson = Newtonsoft.Json.JsonConvert.SerializeObject(actualAttachmentCardResult.Content);

            // ASSERT
            Assert.IsNotNull(actualAttachmentCardResult);
            Assert.AreEqual(expectedCardJson, actualCardJson);
        }
    }
}
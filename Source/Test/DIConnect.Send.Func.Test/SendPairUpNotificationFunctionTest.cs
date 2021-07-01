// <copyright file="SendPairUpNotificationFunctionTest.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Teams.Apps.DIConnect.Send.Func.Test
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.DIConnect.Common.Repositories.UserData;
    using Microsoft.Teams.Apps.DIConnect.Common.Resources;
    using Microsoft.Teams.Apps.DIConnect.Common.Services;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.MessageQueues.UserPairupQueue;
    using Microsoft.Teams.Apps.DIConnect.Common.Services.Teams;
    using Moq;
    using Newtonsoft.Json;
    using Xunit;

    /// <summary>
    /// SendPairUpNotificationFunction test class.
    /// </summary>
    public class SendPairUpNotificationFunctionTest
    {
        private readonly Mock<IMessageService> messageService = new Mock<IMessageService>();
        private readonly Mock<IUserDataRepository> userDataRepository = new Mock<IUserDataRepository>();
        private readonly Mock<IAppSettingsService> appSettingsService = new Mock<IAppSettingsService>();
        private readonly Mock<IMemoryCache> memoryCache = new Mock<IMemoryCache>();
        private Mock<IStringLocalizer<Strings>> localizer = new Mock<IStringLocalizer<Strings>>();

        /// <summary>
        /// Constructor tests.
        /// </summary>
        [Fact]
        public void SendPairUpNotificationFunctionConstructorTest()
        {
            // Arrange
            Action action1 = () => new SendPairUpNotificationFunction(null /*messageService*/, this.userDataRepository.Object, this.appSettingsService.Object, this.memoryCache.Object, this.localizer.Object);
            Action action2 = () => new SendPairUpNotificationFunction(this.messageService.Object, null /*userDataRepository*/, this.appSettingsService.Object, this.memoryCache.Object, this.localizer.Object);
            Action action3 = () => new SendPairUpNotificationFunction(this.messageService.Object, this.userDataRepository.Object, null /*appSettingsService*/, this.memoryCache.Object, this.localizer.Object);
            Action action4 = () => new SendPairUpNotificationFunction(this.messageService.Object, this.userDataRepository.Object, this.appSettingsService.Object, null /*memoryCache*/, this.localizer.Object);
            Action action5 = () => new SendPairUpNotificationFunction(this.messageService.Object, this.userDataRepository.Object, this.appSettingsService.Object, this.memoryCache.Object, null /*localizer*/);
            Action action6 = () => new SendPairUpNotificationFunction(this.messageService.Object, this.userDataRepository.Object, this.appSettingsService.Object, this.memoryCache.Object, this.localizer.Object);

            // Act and Assert.
            action1.Should().Throw<ArgumentNullException>("messageService is null.");
            action2.Should().Throw<ArgumentNullException>("userDataRepository is null.");
            action3.Should().Throw<ArgumentNullException>("appSettingsService is null.");
            action4.Should().Throw<ArgumentNullException>("memoryCache is null.");
            action5.Should().Throw<ArgumentNullException>("localizer is null.");
            action6.Should().NotThrow();
        }

        /// <summary>
        /// Send pair up notification from the bot.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task SendPairUpNotificationFunctionSuccessTest()
        {
            // Arrange
            var sendPairUpNotificationFunctionInstance = this.GetSendPairUpNotificationFunction();
            string data = "{\"PairUpNotificationId\":\"pairUpNotificationId\",\"TeamId\":\"teamId\",\"TeamName\":\"teamName\",\"PairUpUserData\": {\"Recipient1\" : { \"UserPrincipalName\" : \"userPrincipalName\",\"UserGivenName\":\"userGivenName\",\"UserObjectId\":\"userObjectId\"}, \"Recipient2\" : { \"UserPrincipalName\" : \"userPrincipalName\",\"UserGivenName\":\"userGivenName\",\"UserObjectId\":\"userObjectId\"}}}";
            UserPairUpQueueMessageContent messageContent = JsonConvert.DeserializeObject<UserPairUpQueueMessageContent>(data);
            string partitionKey = "UserData";
            string rowKey = "userObjectId";
            string conversationId = "a:hsifswfsfni-bdjebr3e2be3eb2b1k1k12wnk-igueigeugjegjtgjgjgjotirgjoiretgjoitrt-xf";
            Mock<ILogger> logger = new Mock<ILogger>();

            UserDataEntity userDataEntity = new UserDataEntity()
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
                ConversationId = conversationId,
            };
            ExecutionContext executionContext = new ExecutionContext()
            {
                FunctionDirectory = "gafdasvd",
                FunctionName = "dqwpuequ",
                InvocationId = Guid.Empty,
            };

            SendMessageResponse sendMessageResponse = new SendMessageResponse()
            {
                ResultType = SendMessageResult.Succeeded,
            };

            this.localizer
                .Setup(x => x[It.IsAny<string>()])
                .Returns(new LocalizedString("MeetupTitle", "MeetupTitle"));
            this.localizer
                .Setup(x => x[It.IsAny<string>(), It.IsAny<object[]>()])
                .Returns(new LocalizedString("MeetupContent", "MeetupContent"));
            this.memoryCache
                .Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns(Mock.Of<ICacheEntry>);

            this.appSettingsService
                .Setup(x => x.GetServiceUrlAsync())
                .Returns(Task.FromResult("https://www.abc.com"));
            this.userDataRepository
                .Setup(x => x.GetAsync(partitionKey, rowKey))
                .Returns(Task.FromResult(userDataEntity));
            this.messageService
                .Setup(x => x.SendMessageAsync(It.IsAny<IMessageActivity>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), logger.Object))
                .ReturnsAsync(sendMessageResponse);

            // Act
            Func<Task> task = async () => await sendPairUpNotificationFunctionInstance.Run(data, logger.Object, executionContext);

            // Assert
            await task.Should().NotThrowAsync();
            this.appSettingsService.Verify(x => x.GetServiceUrlAsync());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SendPairUpNotificationFunction"/> class.
        /// </summary>
        private SendPairUpNotificationFunction GetSendPairUpNotificationFunction()
        {
            return new SendPairUpNotificationFunction(this.messageService.Object, this.userDataRepository.Object, this.appSettingsService.Object, this.memoryCache.Object, this.localizer.Object);
        }
    }
}
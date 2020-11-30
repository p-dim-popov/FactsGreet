namespace FactsGreet.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using FactsGreet.Data.Common.Repositories;
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Data.Implementations;
    using FactsGreet.Services.Data.Tests.DataHelpers;
    using FactsGreet.Services.Data.Tests.Models.Messages;
    using MockQueryable.Moq;
    using Moq;
    using Xunit;

    public class MessagesServiceTests : Tests<MessagesService>
    {
        [Fact]
        public async Task GetByIdAsync_ShouldWorkAsExpected_WhenExistingIdIsPassed()
        {
            var list = this.GetMessages().OrderBy(_ => this.Random.Next()).ToList();
            var repo = new Mock<IDeletableEntityRepository<Message>>();
            repo.Setup(r => r.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new MessagesService(repo.Object);
            var guid = list
                .OrderBy(_ => this.Random.Next())
                .First().Id;

            var message = await service.GetByIdAsync<MessageWithIdModel>(guid);

            Assert.Equal(guid, message.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExistingIdIsPassed()
        {
            var list = this.GetMessages().OrderBy(_ => this.Random.Next()).ToList();
            var repo = new Mock<IDeletableEntityRepository<Message>>();
            repo.Setup(r => r.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new MessagesService(repo.Object);
            var guid = Guid.Empty;

            var message = await service.GetByIdAsync<MessageWithIdModel>(guid);

            Assert.Null(message);
        }

        [Fact]
        public async Task CanUserGetMessageAsync_ShouldReturnFalse_WhenUserCannot()
        {
            var list = this.GetMessages().OrderBy(_ => this.Random.Next()).ToList();
            var message = list.First();
            var repo = new Mock<IDeletableEntityRepository<Message>>();
            repo.Setup(r => r.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new MessagesService(repo.Object);

            Assert.False(await service.CanUserGetMessageAsync(message.Id, string.Empty));
        }

        [Fact]
        public async Task CanUserGetMessageAsync_ShouldReturnTrue_WhenUserCan()
        {
            var list = this.GetMessages().OrderBy(_ => this.Random.Next()).ToList();
            var message = list.First();
            var repo = new Mock<IDeletableEntityRepository<Message>>();
            repo.Setup(r => r.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new MessagesService(repo.Object);

            Assert.True(await service.CanUserGetMessageAsync(message.Id, message.Conversation.Users.First().Id));
        }

        [Fact]
        public async Task GetFewOlderThanAsync_ShouldReturnMessagesOnlyFromConversationId()
        {
            var list = this.GetMessagesFromConversation1()
                .Concat(this.GetMessagesFromConversation2())
                .OrderBy(_ => this.Random.Next())
                .ToList();
            var message = list.First();
            var repo = new Mock<IDeletableEntityRepository<Message>>();
            repo.Setup(r => r.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new MessagesService(repo.Object);

            var fewOlder =
                await service.GetFewOlderThanAsync<MessageWithConversation>(message.Id, message.Conversation.Id, 3);

            Assert.All(fewOlder, x => Assert.True(x.Conversation.Id == message.Conversation.Id));
        }

        [Fact]
        public async Task
            GetFewOlderThanAsync_ShouldReturn3MessagesOlderThanReferencedMessageCreationDate_WhenReferenceMessageHasValue()
        {
            var list = this.GetMessagesFromConversation1().OrderBy(_ => this.Random.Next()).ToList();
            var message = list.First();
            var repo = new Mock<IDeletableEntityRepository<Message>>();
            repo.Setup(r => r.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new MessagesService(repo.Object);

            var fewOlder =
                await service.GetFewOlderThanAsync<MessageWithCreatedOn>(message.Id, message.Conversation.Id, 3);

            Assert.All(fewOlder, x => Assert.True(x.CreatedOn < message.CreatedOn));
            Assert.Equal(fewOlder, fewOlder.OrderByDescending(x => x.CreatedOn));
        }
    }
}

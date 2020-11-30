namespace FactsGreet.Services.Data.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using FactsGreet.Data.Common.Repositories;
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;
    using Microsoft.EntityFrameworkCore;

    public class MessagesService : IMessagesService
    {
        private readonly IDeletableEntityRepository<Message> messageRepository;

        public MessagesService(
            IDeletableEntityRepository<Message> messageRepository)
        {
            this.messageRepository = messageRepository;
        }

        public Task<T> GetByIdAsync<T>(Guid messageId)
            where T : IMapFrom<Message>
        {
            return this.messageRepository
                .AllAsNoTracking()
                .Where(x => x.Id == messageId)
                .To<T>()
                .FirstOrDefaultAsync();
        }

        public Task<bool> CanUserGetMessageAsync(Guid messageId, string userId)
        {
            return this.messageRepository.AllAsNoTracking()
                .Where(x => x.Id == messageId)
                .Select(x => x.Conversation)
                .SelectMany(x => x.Users)
                .AnyAsync(x => x.Id == userId);
        }

        public async Task<ICollection<T>> GetFewOlderThanAsync<T>(
            Guid? referenceMessageId,
            Guid conversationId,
            int take)
            where T : IMapFrom<Message>
        {
            var query = this.messageRepository
                .AllAsNoTracking()
                .Where(x => x.ConversationId == conversationId);

            if (referenceMessageId.HasValue)
            {
                var referenceDate = await this.messageRepository.AllAsNoTracking()
                    .Where(x => x.Id == referenceMessageId)
                    .Select(x => x.CreatedOn)
                    .FirstOrDefaultAsync();

                query = query.Where(x => x.CreatedOn < referenceDate);
            }

            return await query
                .OrderByDescending(x => x.CreatedOn)
                .Take(take)
                .To<T>()
                .ToListAsync();
        }
    }
}

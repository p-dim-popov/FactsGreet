namespace FactsGreet.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public interface IMessagesService
    {
        public Task<T> GetByIdAsync<T>(Guid messageId)
            where T : IMapFrom<Message>;

        public Task<bool> CanUserGetMessageAsync(Guid messageId, string userId);

        public Task<ICollection<T>> GetFewOlderThanAsync<T>(
            Guid? referenceMessageId,
            Guid conversationId,
            int take)
            where T : IMapFrom<Message>;
    }
}

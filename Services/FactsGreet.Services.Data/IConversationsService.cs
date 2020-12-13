namespace FactsGreet.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public interface IConversationsService
    {
        Task<T> GetAsync<T>(params string[] userIds)
            where T : IMapFrom<Conversation>;

        Task<T> GetAsync<T>(Guid id)
            where T : IMapFrom<Conversation>;

        Task<Guid> SendMessageAsync(string senderId, Guid conversationId, string content);

        Task<T> CreateAsync<T>(params string[] userIds)
            where T : IMapFrom<Conversation>;

        Task<bool> IsUserParticipantAsync(Guid id, string userId);

        Task<ICollection<string>> GetParticipantsAsync(Guid id);

        Task<ICollection<T>> GetFewOlderThanAsync<T>(
            Guid? referenceConversationId, string userId, int take)
            where T : IMapFrom<Conversation>;

        Task<string> GetTitleAsync(Guid id);

        Task<ICollection<T>> GetByUsersEmailsAsync<T>(IEnumerable<string> usersEmails)
            where T : IMapFrom<Conversation>;
    }
}

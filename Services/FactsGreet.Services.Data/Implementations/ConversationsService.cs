namespace FactsGreet.Services.Data.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using FactsGreet.Common;
    using FactsGreet.Data.Common.Repositories;
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;
    using Microsoft.EntityFrameworkCore;

    public class ConversationsService
    {
        private readonly IDeletableEntityRepository<Message> messageRepository;
        private readonly IDeletableEntityRepository<Conversation> conversationRepository;
        private readonly IDeletableEntityRepository<ApplicationUser> applicationUserRepository;

        public ConversationsService(
            IDeletableEntityRepository<Message> messageRepository,
            IDeletableEntityRepository<Conversation> conversationRepository,
            IDeletableEntityRepository<ApplicationUser> applicationUserRepository)
        {
            this.messageRepository = messageRepository;
            this.conversationRepository = conversationRepository;
            this.applicationUserRepository = applicationUserRepository;
        }

        public async Task<T> GetAsync<T>(params string[] userIds)
            where T : IMapFrom<Conversation>
        {
            userIds = userIds.Distinct().ToArray();
            return await this.conversationRepository.AllAsNoTracking()
                .Where(x => x.Users.Count == userIds.Length &&
                            x.Users.All(y => userIds.Contains(y.Id)))
                .To<T>()
                .FirstOrDefaultAsync();
        }

        public async Task<T> GetAsync<T>(Guid id)
            where T : IMapFrom<Conversation>
        {
            return await this.conversationRepository.AllAsNoTracking()
                .Where(x => x.Id == id)
                .To<T>()
                .FirstOrDefaultAsync();
        }

        public async Task<Guid> SendMessageAsync(string senderId, Guid conversationId, string content)
        {
            var conversation = await this.conversationRepository.All()
                .FirstOrDefaultAsync(x => x.Id == conversationId);

            var message = new Message { SenderId = senderId, Content = content };
            conversation.Messages.Add(message);

            await this.conversationRepository.SaveChangesAsync();

            return message.Id;
        }

        public async Task<T> CreateAsync<T>(params string[] userIds)
            where T : class, IMapFrom<Conversation>
        {
            userIds = userIds.Distinct().ToArray();
            var users = await this.applicationUserRepository
                .All()
                .Where(x => userIds.Contains(x.Id))
                .ToListAsync();

            var conversation = new Conversation
            {
                Title = users.Join(", "),
                Users = users,
                Messages = new List<Message>
                {
                    new Message
                    {
                        Content = "Say hello to each other :) !",
                        SenderId = await this.applicationUserRepository.AllAsNoTracking()
                            .Where(x => x.UserName == "admin@localhost")
                            .Select(x => x.Id)
                            .FirstOrDefaultAsync(),
                    },
                },
            };

            await this.conversationRepository.AddAsync(conversation);
            await this.conversationRepository.SaveChangesAsync();

            return new MapperConfiguration(cfg => cfg.CreateMap<Conversation, T>())
                .CreateMapper()
                .Map<Conversation, T>(conversation);
        }

        public Task<bool> IsUserParticipantAsync(Guid id, string userId)
        {
            return this.conversationRepository.AllAsNoTracking()
                .Where(x => x.Id == id)
                .SelectMany(x => x.Users)
                .Select(x => x.Id)
                .AnyAsync(x => x == userId);
        }

        public async Task<ICollection<string>> GetParticipantsAsync(Guid id)
        {
            return await this.conversationRepository
                .AllAsNoTracking()
                .Where(x => x.Id == id)
                .SelectMany(x => x.Users)
                .Select(x => x.Id)
                .ToListAsync();
        }

        public async Task<ICollection<T>> GetFewOlderThanAsync<T>(Guid? referenceConversationId, string userId,
            int take)
            where T : IMapFrom<Conversation>
        {
            var query = this.applicationUserRepository
                .AllAsNoTracking()
                .Where(x => x.Id == userId)
                .SelectMany(x => x.Conversations);

            if (referenceConversationId.HasValue)
            {
                var referenceDate = await this.conversationRepository
                    .AllAsNoTracking()
                    .Where(x => x.Id == referenceConversationId)
                    .Select(x => x.Messages
                        .OrderByDescending(y => y.CreatedOn)
                        .FirstOrDefault()
                        .CreatedOn)
                    .FirstOrDefaultAsync();

                query = query.Where(x => x.Messages
                    .OrderByDescending(y => y.CreatedOn)
                    .FirstOrDefault()
                    .CreatedOn < referenceDate);
            }

            return await query
                .OrderByDescending(x => x.Messages
                    .OrderByDescending(y => y.CreatedOn)
                    .FirstOrDefault()
                    .CreatedOn)
                .Take(take)
                .To<T>()
                .ToListAsync();
        }

        public Task<string> GetTitleAsync(Guid id)
            => this.conversationRepository.AllAsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => x.Title)
                .FirstOrDefaultAsync();

        public async Task<ICollection<T>> GetByUsersEmailsAsync<T>(IEnumerable<string> usersEmails)
        {
            usersEmails = usersEmails.Select(x => x.ToUpper()).ToList();
            return await usersEmails.Aggregate(
                    this.conversationRepository.AllAsNoTracking(),
                    (acc, cur) => acc
                        .Where(x => x.Users
                            .Any(y => y.NormalizedEmail
                                .StartsWith(cur))))
                // .Where(x => x.Users
                //     .Count(y => usersEmails
                //         .Contains(y.NormalizedEmail)) >= usersEmails.Count())

                // .Where(x => x.Users.Count(y => y.NormalizedEmail.StartsWith()))

                // BUG: For unknown reason, when orderby was initialy used, made results not correct,
                // when removed and later used again, no bugs caused
                .OrderBy(x => x.Users.Count)
                .To<T>()
                .ToListAsync();
        }
    }
}

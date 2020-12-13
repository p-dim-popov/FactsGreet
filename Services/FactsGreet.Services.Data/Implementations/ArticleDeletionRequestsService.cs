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

    public class ArticleDeletionRequestsService : IArticleDeletionRequestsService
    {
        private readonly IRepository<ArticleDeletionRequest> articleDeletionRequestRepository;

        public ArticleDeletionRequestsService(IRepository<ArticleDeletionRequest> articleDeletionRequestRepository)
        {
            this.articleDeletionRequestRepository = articleDeletionRequestRepository;
        }

        public async Task<ICollection<T>> GetPaginatedOrderedByCreationDateAsync<T>(int skip, int take)
            where T : IMapFrom<ArticleDeletionRequest>
            => await this.articleDeletionRequestRepository
                .AllAsNoTracking()
                .OrderByDescending(x => x.CreatedOn)
                .Skip(skip)
                .Take(take)
                .To<T>()
                .ToListAsync();

        public Task<int> GetCountAsync()
            => this.articleDeletionRequestRepository
                .AllAsNoTracking()
                .CountAsync();

        public Task<T> GetById<T>(Guid id)
            where T : IMapFrom<ArticleDeletionRequest>
            => this.articleDeletionRequestRepository
                .AllAsNoTracking()
                .Where(x => x.Id == id)
                .To<T>()
                .FirstOrDefaultAsync();
    }
}

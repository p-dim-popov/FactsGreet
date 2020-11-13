namespace FactsGreet.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using FactsGreet.Data.Common.Repositories;
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;
    using Microsoft.EntityFrameworkCore;

    public class ArticleDeletionRequestsService
    {
        private readonly IRepository<ArticleDeletionRequest> aricleDeletionRequestRepository;

        public ArticleDeletionRequestsService(IRepository<ArticleDeletionRequest> aricleDeletionRequestRepository)
        {
            this.aricleDeletionRequestRepository = aricleDeletionRequestRepository;
        }

        public async Task<ICollection<T>> GetPaginatedOrderedByCreationDateAsync<T>(int skip, int take)
            => await this.aricleDeletionRequestRepository.AllAsNoTracking()
                .OrderByDescending(x => x.CreatedOn)
                .Skip(skip)
                .Take(take)
                .To<T>()
                .ToListAsync();

        public Task<int> GetCountAsync()
            => this.aricleDeletionRequestRepository.AllAsNoTracking().CountAsync();

        public Task<T> GetById<T>(Guid id)
            => this.aricleDeletionRequestRepository.All()
                .Where(x => x.Id == id)
                .To<T>()
                .FirstOrDefaultAsync();
    }
}
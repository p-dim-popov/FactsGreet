namespace FactsGreet.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public interface IArticleDeletionRequestsService
    {
        Task<ICollection<T>> GetPaginatedOrderedByCreationDateAsync<T>(int skip, int take)
            where T : IMapFrom<ArticleDeletionRequest>;

        Task<int> GetCountAsync();

        Task<T> GetById<T>(Guid id)
            where T : IMapFrom<ArticleDeletionRequest>;
    }
}

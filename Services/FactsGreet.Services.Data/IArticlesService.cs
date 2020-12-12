namespace FactsGreet.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public interface IArticlesService
    {
        Task<string> GetTitleAsync(Guid id);

        Task CreateAsync(
            string authorId,
            string title,
            string content,
            string[] categories,
            string thumbnailLink,
            string description);

        Task<ICollection<T>> GetPaginatedByTitleKeywordsAsync<T>(int skip, int take, string keywords)
            where T : IMapFrom<Article>;

        Task<int> GetCountByTitleKeywordsAsync(string keywords);

        Task<ICollection<T>> GetPaginatedOrderedByDescAsync<T, TOrderKey>(
            int skip,
            int take,
            Expression<Func<Article, TOrderKey>> order)
            where T : IMapFrom<Article>;

        Task<T> GetByTitleAsync<T>(string title)
            where T : IMapFrom<Article>;

        Task<int> GetCountAsync();

        Task<bool> DoesTitleExistAsync(string title);

        Task CreateDeletionRequestAsync(Guid id, string userId, string reason);

        Task DeleteAsync(Guid id);

        Task<string> GetAuthorIdAsync(Guid id);

        Task<T> GetByIdAsync<T>(Guid id)
            where T : IMapFrom<Article>;
    }
}

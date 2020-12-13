namespace FactsGreet.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public interface IStarsService
    {
        Task CreateStarLinkAsync(string userId, Guid articleId);

        Task RemoveStarLinkAsync(string userId, Guid articleId);

        Task<bool> IsArticleStarredByUserAsync(Guid articleId, string userId);

        Task<ICollection<T>> GetAllStarredByUser<T>(string userId)
            where T : IMapFrom<Article>;
    }
}

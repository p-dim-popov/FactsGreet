namespace FactsGreet.Services.Data
{
    using System;
    using System.Threading.Tasks;

    public interface IStarsService
    {
        Task CreateStarLinkAsync(string userId, Guid articleId);

        Task RemoveStarLinkAsync(string userId, Guid articleId);

        Task<bool> IsArticleStarredByUserAsync(Guid articleId, string userId);
    }
}

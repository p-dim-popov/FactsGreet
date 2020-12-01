namespace FactsGreet.Services.Data
{
    using System;
    using System.Threading.Tasks;

    public interface IStarsService
    {
        public Task CreateStarLinkAsync(string userId, Guid articleId);

        public Task RemoveStarLinkAsync(string userId, Guid articleId);
    }
}

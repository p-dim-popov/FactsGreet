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

    public class StarsService : IStarsService
    {
        private readonly IDeletableEntityRepository<Star> starRepository;

        public StarsService(IDeletableEntityRepository<Star> starRepository)
        {
            this.starRepository = starRepository;
        }

        public async Task CreateStarLinkAsync(string userId, Guid articleId)
        {
            var star = await this.starRepository.AllWithDeleted()
                .FirstOrDefaultAsync(x => x.UserId == userId && x.ArticleId == articleId);

            if (star is null)
            {
                await this.starRepository.AddAsync(new Star { UserId = userId, ArticleId = articleId });
            }
            else if (star.IsDeleted)
            {
                this.starRepository.Undelete(star);
            }
            else
            {
                return;
            }

            await this.starRepository.SaveChangesAsync();
        }

        public async Task RemoveStarLinkAsync(string userId, Guid articleId)
        {
            var star = await this.starRepository.All()
                .FirstOrDefaultAsync(x => x.UserId == userId && x.ArticleId == articleId);

            if (star is not null)
            {
                this.starRepository.Delete(star);
                await this.starRepository.SaveChangesAsync();
            }
        }

        public Task<bool> IsArticleStarredByUserAsync(Guid articleId, string userId)
            => this.starRepository.AllAsNoTracking()
                .AnyAsync(x => x.ArticleId == articleId && x.UserId == userId);

        public async Task<ICollection<T>> GetAllStarredByUser<T>(string userId)
            where T : IMapFrom<Article>
            => await this.starRepository
                .AllAsNoTracking()
                .Where(x => x.UserId == userId)
                .Select(x => x.Article)
                .To<T>()
                .ToListAsync();
    }
}

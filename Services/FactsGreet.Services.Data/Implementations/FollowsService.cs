namespace FactsGreet.Services.Data.Implementations
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using FactsGreet.Data.Common.Repositories;
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;
    using Microsoft.EntityFrameworkCore;

    public class FollowsService : IFollowsService
    {
        private readonly IDeletableEntityRepository<Follow> followRepository;

        public FollowsService(IDeletableEntityRepository<Follow> followRepository)
        {
            this.followRepository = followRepository;
        }

        public async Task FollowAsync(string followerId, string followedId)
        {
            var follow = await this.followRepository.AllWithDeleted()
                .FirstOrDefaultAsync(x => x.FollowerId == followerId && x.FollowedId == followedId);

            if (follow is null)
            {
                await this.followRepository.AddAsync(new Follow { FollowerId = followerId, FollowedId = followedId });
            }
            else if (follow.IsDeleted)
            {
                this.followRepository.Undelete(follow);
            }
            else
            {
                return;
            }

            await this.followRepository.SaveChangesAsync();
        }

        public async Task UnfollowAsync(string followerId, string followedId)
        {
            var follow = await this.followRepository.All()
                .FirstOrDefaultAsync(x => x.FollowerId == followerId && x.FollowedId == followedId);

            if (follow is not null)
            {
                this.followRepository.Delete(follow);
                await this.followRepository.SaveChangesAsync();
            }
        }

        public Task<bool> IsUserFollowingUserAsync(string followerId, string followedId)
            => this.followRepository.AllAsNoTracking()
                .AnyAsync(x => x.FollowerId == followerId && x.FollowedId == followedId);

        public async Task<ICollection<T>> GetFollowedUsersAsync<T>(string userId)
            where T : IMapFrom<ApplicationUser>
            => await this.followRepository.AllAsNoTracking()
                .Where(x => x.FollowerId == userId)
                .Select(x => x.Followed)
                .To<T>()
                .ToListAsync();
    }
}

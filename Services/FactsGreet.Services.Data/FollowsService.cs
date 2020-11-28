namespace FactsGreet.Services.Data
{
    using System.Threading.Tasks;

    using FactsGreet.Data.Common.Repositories;
    using FactsGreet.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class FollowsService
    {
        private readonly IDeletableEntityRepository<Follow> followRepository;

        public FollowsService(IDeletableEntityRepository<Follow> followRepository)
        {
            this.followRepository = followRepository;
        }

        public async Task Follow(string followerId, string followedId)
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

        public async Task Unfollow(string followerId, string followedId)
        {
            var star = await this.followRepository.All()
                .FirstOrDefaultAsync(x => x.FollowerId == followerId && x.FollowedId == followedId);

            if (star is not null)
            {
                this.followRepository.Delete(star);
                await this.followRepository.SaveChangesAsync();
            }
        }

        public Task<bool> IsUserFollowingUserAsync(string followerId, string followedId)
            => this.followRepository.AllAsNoTracking()
                .AnyAsync(x => x.FollowerId == followerId && x.FollowedId == followedId);
    }
}

namespace FactsGreet.Services.Data.Implementations
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using FactsGreet.Data.Common.Repositories;
    using FactsGreet.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class BadgesService : IBadgesService
    {
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;
        private readonly IRepository<Badge> badgeRepository;

        public BadgesService(
            IDeletableEntityRepository<ApplicationUser> userRepository,
            IRepository<Badge> badgeRepository)
        {
            this.userRepository = userRepository;
            this.badgeRepository = badgeRepository;
        }

        public async Task<ICollection<Badge>> GetAllAsync()
            => await this.badgeRepository
                .AllAsNoTracking()
                .ToListAsync();

        public async Task RemoveBadgeFromUserAsync(string userId, string badgeName)
        {
            var user = await this.userRepository
                .All()
                .Include(x => x.Badges.Where(y => y.Name == badgeName))
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user.Badges.FirstOrDefault(x => x.Name == badgeName) is { } badge)
            {
                user.Badges.Remove(badge);
                await this.userRepository.SaveChangesAsync();
            }
        }

        public async Task AddBadgeToUserAsync(string userId, string name)
        {
            var user = await this.userRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == userId);

            var badge = await this.badgeRepository.All()
                .FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());

            if (user is not null && badge is not null)
            {
                user.Badges.Add(badge);
                await this.userRepository.SaveChangesAsync();
            }
        }
    }
}

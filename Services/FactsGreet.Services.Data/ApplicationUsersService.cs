namespace FactsGreet.Services.Data
{
    using System.Linq;
    using System.Threading.Tasks;

    using FactsGreet.Data.Common.Repositories;
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;
    using Microsoft.EntityFrameworkCore;

    public class ApplicationUsersService
    {
        private readonly IDeletableEntityRepository<ApplicationUser> applicationUserRepository;

        public ApplicationUsersService(IDeletableEntityRepository<ApplicationUser> applicationUserRepository)
        {
            this.applicationUserRepository = applicationUserRepository;
        }

        public Task<T> GetByEmailAsync<T>(string email)
            => this.applicationUserRepository.AllAsNoTracking()
                .Where(x => x.Email == email)
                .To<T>()
                .FirstOrDefaultAsync();

        public async Task RemoveBadgeAsync(string userId, string badgeName)
        {
            var user = await this.applicationUserRepository.All()
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user.Badges.FirstOrDefault(x => x.Name == badgeName) is { } badge)
            {
                user.Badges.Remove(badge);
                await this.applicationUserRepository.SaveChangesAsync();
            }
        }
    }
}
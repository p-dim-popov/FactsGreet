namespace FactsGreet.Services.Data
{
    using System.Collections.Generic;
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

        public Task<string> GetEmailAsync(string id)
            => this.applicationUserRepository.AllAsNoTrackingWithDeleted()
                .Where(x => x.Id == id)
                .Select(x => x.Email)
                .FirstOrDefaultAsync();

        public Task<string> GetIdByEmailAsync(string email)
        {
            return this.applicationUserRepository.AllAsNoTracking()
                .Where(x => x.Email == email)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();
        }

        public Task<bool> EmailExistsAsync(string last)
        {
            last = last.ToUpper();
            return this.applicationUserRepository
                .AllAsNoTracking()
                .AnyAsync(x => x.NormalizedEmail == last);
        }

        public async Task<ICollection<string>> Get10EmailsByEmailKeywordAsync(string keyword)
        {
            keyword = keyword.ToUpperInvariant();
            return await this.applicationUserRepository
                .AllAsNoTracking()
                .Where(x => x.NormalizedEmail.StartsWith(keyword))
                .Select(x => x.Email)
                .OrderBy(x => x.Length)
                .Take(10)
                .ToListAsync();
        }
    }
}

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

        public async Task<ICollection<T>> Get10MostActiveUsersForTheLastWeekAsync<T>()
        {
            var lastWeek = DateTime.UtcNow.AddDays(-7);
            return await this.applicationUserRepository
                .AllAsNoTracking()
                .OrderByDescending(x => x
                    .Edits.Count(y => y.CreatedOn > lastWeek))
                .ThenByDescending(x => x.Edits
                    .Where(y => y.IsCreation)
                    .Count(y => y.CreatedOn > lastWeek))
                .Take(10)
                .To<T>()
                .ToListAsync();
        }
    }
}

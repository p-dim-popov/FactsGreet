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

    public class AdminRequestsService
    {
        private readonly IDeletableEntityRepository<AdminRequest> adminRequestRepository;
        private readonly IRepository<Badge> badgeRepository;

        public AdminRequestsService(
            IDeletableEntityRepository<AdminRequest> adminRequestRepository,
            IRepository<Badge> badgeRepository)
        {
            this.adminRequestRepository = adminRequestRepository;
            this.badgeRepository = badgeRepository;
        }

        public Task<int> GetCountAsync()
            => this.adminRequestRepository
                .AllAsNoTracking()
                .CountAsync();

        public async Task<bool> CanUserBecomeAdminAsync(string userId)
        {
            var badgesCount = await this.GetCountAsync();

            var userBadges = await this.badgeRepository.AllAsNoTracking()
                .SelectMany(x => x.UsersWithBadges)
                .Where(x => x.Id == userId)
                .SelectMany(x => x.Badges)
                .CountAsync();
            return userBadges > (badgesCount / 2);
        }

        public async Task CreateAsync(string userId, string motivationalLetter)
        {
            if (await this.adminRequestRepository
                .AllAsNoTracking()
                .AnyAsync(x => x.Request.SenderId == userId))
            {
                return;
            }

            await this.adminRequestRepository.AddAsync(new AdminRequest
                { Request = { SenderId = userId }, MotivationalLetter = motivationalLetter });
            await this.adminRequestRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var request = await this.adminRequestRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == id);
            this.adminRequestRepository.HardDelete(request);
            await this.adminRequestRepository.SaveChangesAsync();
        }

        public async Task DeleteForUserIdAsync(string userId)
        {
            var request = await this.adminRequestRepository
                .All()
                .FirstOrDefaultAsync(x => x.Request.SenderId == userId);
            this.adminRequestRepository.HardDelete(request);
            await this.adminRequestRepository.SaveChangesAsync();
        }

        public async Task<ICollection<T>> GetPaginatedOrderedByCreationDateAsync<T>(int skip, int take)
        {
            return await this.adminRequestRepository
                .AllAsNoTracking()
                .OrderByDescending(x => x.CreatedOn)
                .Skip(skip)
                .Take(take)
                .To<T>()
                .ToListAsync();
        }

        public Task<T> GetById<T>(Guid id)
            => this.adminRequestRepository
                .AllAsNoTracking()
                .Where(x => x.Id == id)
                .To<T>()
                .FirstOrDefaultAsync();
    }
}

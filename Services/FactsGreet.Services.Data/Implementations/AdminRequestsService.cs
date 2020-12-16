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

    public class AdminRequestsService : IAdminRequestsService
    {
        private readonly IDeletableEntityRepository<AdminRequest> adminRequestRepository;
        private readonly IBadgesService badgesService;

        public AdminRequestsService(
            IDeletableEntityRepository<AdminRequest> adminRequestRepository,
            IBadgesService badgesService)
        {
            this.adminRequestRepository = adminRequestRepository;
            this.badgesService = badgesService;
        }

        public Task<int> GetCountAsync()
            => this.adminRequestRepository
                .AllAsNoTracking()
                .CountAsync();

        public async Task<bool> CanUserBecomeAdminAsync(string userId)
            => await this.badgesService
                .GetUserBadgesCountByUserIdAsync(userId) > await this.badgesService.GetCountAsync() / 2;

        public async Task CreateAsync(string userId, string motivationalLetter)
        {
            if (await this.adminRequestRepository
                .All()
                .FirstOrDefaultAsync(x => x.Request.SenderId == userId) is { } request)
            {
                this.adminRequestRepository.HardDelete(request);
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
            where T : IMapFrom<AdminRequest>
            => await this.adminRequestRepository
                .AllAsNoTracking()
                .OrderByDescending(x => x.CreatedOn)
                .Skip(skip)
                .Take(take)
                .To<T>()
                .ToListAsync();

        public Task<T> GetById<T>(Guid id)
            where T : IMapFrom<AdminRequest>
            => this.adminRequestRepository
                .AllAsNoTracking()
                .Where(x => x.Id == id)
                .To<T>()
                .FirstOrDefaultAsync();
    }
}

namespace FactsGreet.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public interface IAdminRequestsService
    {
        Task<int> GetCountAsync();

        Task<bool> CanUserBecomeAdminAsync(string userId);

        Task CreateAsync(string userId, string motivationalLetter);

        Task DeleteAsync(Guid id);

        Task DeleteForUserIdAsync(string userId);

        Task<ICollection<T>> GetPaginatedOrderedByCreationDateAsync<T>(int skip, int take)
            where T : IMapFrom<AdminRequest>;

        Task<T> GetById<T>(Guid id)
            where T : IMapFrom<AdminRequest>;
    }
}

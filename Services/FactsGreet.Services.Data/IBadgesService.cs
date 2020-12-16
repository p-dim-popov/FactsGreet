namespace FactsGreet.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using FactsGreet.Data.Models;

    public interface IBadgesService
    {
        Task<ICollection<Badge>> GetAllAsync();

        Task RemoveBadgeFromUserAsync(string userId, string badgeName);

        Task AddBadgeToUserAsync(string userId, string name);

        Task<int> GetCountAsync();

        Task<int> GetUserBadgesCountByUserIdAsync(string userId);
    }
}

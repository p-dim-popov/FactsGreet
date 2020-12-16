namespace FactsGreet.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public interface IFollowsService
    {
        Task FollowAsync(string followerId, string followedId);

        Task UnfollowAsync(string followerId, string followedId);

        Task<bool> IsUserFollowingUserAsync(string followerId, string followedId);

        Task<ICollection<T>> GetFollowedUsersAsync<T>(string userId)
            where T : IMapFrom<ApplicationUser>;
    }
}

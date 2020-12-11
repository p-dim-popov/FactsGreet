namespace FactsGreet.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public interface IApplicationUsersService
    {
        Task<T> GetByEmailAsync<T>(string email)
            where T : IMapFrom<ApplicationUser>;

        Task<string> GetEmailAsync(string id);

        Task<string> GetIdByEmailAsync(string email);

        Task<ICollection<string>> Get10EmailsByEmailKeywordAsync(string keyword);

        Task<ICollection<T>> Get10MostActiveUsersForTheLastWeekAsync<T>();
    }
}

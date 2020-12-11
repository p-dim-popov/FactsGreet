namespace FactsGreet.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public interface IApplicationUsersService
    {
        public Task<T> GetByEmailAsync<T>(string email)
            where T : IMapFrom<ApplicationUser>;

        public Task<string> GetEmailAsync(string id);

        public Task<string> GetIdByEmailAsync(string email);

        public Task<ICollection<string>> Get10EmailsByEmailKeywordAsync(string keyword);

        public Task<ICollection<T>> Get10MostActiveUsersForTheLastWeekAsync<T>();
    }
}

namespace FactsGreet.Services.Data.Tests.DataHelpers
{
    using System.Collections.Generic;
    using System.Linq;
    using FactsGreet.Data.Models;

    public static class ApplicationUsersServiceDataHelper
    {
        public static ICollection<ApplicationUser> GetUsers(this ApplicationUsersServiceTests tests)
            => Enumerable.Range(100, 10)
                .Select(x => new ApplicationUser
                {
                    Id = x.ToString(),
                    Email = ((char)x).ToString(),
                    NormalizedEmail = ((char)x).ToString().ToUpper(),
                })
                .ToList();

        public static ICollection<ApplicationUser> GetJinAndJonesAtHome(this ApplicationUsersServiceTests tests, int count = 30)
            => Enumerable.Range(1, count)
                .Select(x => new ApplicationUser
                {
                    Email = (x % 2 == 0 ? $"Jin{x}@home" : $"John{x}@home").ToUpper(),
                    NormalizedEmail = (x % 2 == 0 ? $"Jin{x}@home" : $"John{x}@home").ToUpper(),
                })
                .ToList();
    }
}

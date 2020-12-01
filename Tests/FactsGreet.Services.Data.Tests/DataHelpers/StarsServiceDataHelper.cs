namespace FactsGreet.Services.Data.Tests.DataHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FactsGreet.Data.Models;

    public static class StarsServiceDataHelper
    {
        public static ICollection<Article> GetArticles(this StarsServiceTests tests)
            => Enumerable.Range(1, 3)
                .Select(x => new Article { Id = Guid.NewGuid(), })
                .ToList();

        public static ICollection<ApplicationUser> GetUsers(this StarsServiceTests tests)
            => Enumerable.Range(1, 10)
                .Select(x => new ApplicationUser { Id = Guid.NewGuid().ToString() })
                .ToList();
    }
}

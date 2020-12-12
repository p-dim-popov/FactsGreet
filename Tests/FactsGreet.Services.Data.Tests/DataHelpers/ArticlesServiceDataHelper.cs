namespace FactsGreet.Services.Data.Tests.DataHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FactsGreet.Data.Models;

    public static class ArticlesServiceDataHelper
    {
        public static ICollection<Article> GetArticles(this ArticlesServiceTests tests)
            => Enumerable.Range(1, 10)
                .Select(x => new Article
                {
                    Id = Guid.NewGuid(),
                    Title = $"{nameof(ArticlesServiceDataHelper)} {x} ~ -?",
                    AuthorId = x.ToString(),
                })
                .ToList();
    }
}

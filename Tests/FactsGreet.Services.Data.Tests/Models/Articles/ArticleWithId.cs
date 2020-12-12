namespace FactsGreet.Services.Data.Tests.Models.Articles
{
    using System;

    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public class ArticleWithId : IMapFrom<Article>
    {
        public Guid Id { get; set; }
    }
}

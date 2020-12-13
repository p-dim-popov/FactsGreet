namespace FactsGreet.Services.Data.TransferObjects.Articles
{
    using System;

    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public class ArticleWithId : IMapFrom<Article>
    {
        public Guid Id { get; set; }
    }
}

namespace FactsGreet.Web.ViewModels.Articles
{
    using System;

    using AutoMapper;
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public class ArticleViewModel : IMapFrom<Article>
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public int StarsCount { get; set; }

        public string AuthorId { get; set; }

        [IgnoreMap]
        public bool IsStarredByUser { get; set; }
    }
}
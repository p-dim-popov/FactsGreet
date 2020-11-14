namespace FactsGreet.Web.ViewModels.Administration.Dashboard
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;
    using FactsGreet.Web.ViewModels.Articles;

    public class ArticleDeletionRequestViewModel : IMapFrom<ArticleDeletionRequest>
    {
        public Guid Id { get; set; }

        public CompactArticleViewModel Article { get; set; }

        [Display(Name = "Username")]
        public string ArticleAuthorUserName { get; set; }

        public Guid ArticleId { get; set; }

        public string Reason { get; set; }
    }
}

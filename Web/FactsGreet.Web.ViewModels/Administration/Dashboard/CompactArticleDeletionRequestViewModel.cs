namespace FactsGreet.Web.ViewModels.Administration.Dashboard
{
    using System;

    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public class CompactArticleDeletionRequestViewModel : IMapFrom<ArticleDeletionRequest>
    {
        public string ArticleAuthorUserName { get; set; }

        public string ArticleTitle { get; set; }

        public Guid Id { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
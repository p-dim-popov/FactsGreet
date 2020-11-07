namespace FactsGreet.Web.ViewModels.Articles
{
    using System.Collections.Generic;

    using FactsGreet.Web.ViewModels.Shared;

    public class SearchArticlesViewModel
    {
        public ICollection<CompactArticleViewModel> Articles { get; set; }

        public PaginationViewModel PaginationViewModel { get; set; }

        public string Slug { get; set; }
    }
}
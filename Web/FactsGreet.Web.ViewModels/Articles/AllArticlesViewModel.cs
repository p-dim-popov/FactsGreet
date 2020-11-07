namespace FactsGreet.Web.ViewModels.Articles
{
    using System.Collections.Generic;

    using FactsGreet.Web.ViewModels.Shared;

    public class AllArticlesViewModel
    {
        public ICollection<CompactArticleViewModel> Articles { get; set; }

        public PaginationViewModel PaginationViewModel { get; set; }
    }
}
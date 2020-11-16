namespace FactsGreet.Web.ViewModels.Edits
{
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;
    using FactsGreet.Web.ViewModels.Articles;
    using FactsGreet.Web.ViewModels.Shared;

    public class HistoryViewModel : IMapFrom<Article>
    {
        public ArticleWithEditsViewModel Article { get; set; }

        public CompactPaginationViewModel PaginationViewModel { get; set; }
    }
}

namespace FactsGreet.Web.ViewModels.Edits
{
    using System.Collections.Generic;

    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;
    using FactsGreet.Web.ViewModels.Shared;

    public class HistoryViewModel : IMapFrom<Article>
    {
        public CompactPaginationViewModel PaginationViewModel { get; set; }

        public string ArticleTitle { get; set; }

        public ICollection<CompactEditViewModel> Edits { get; set; }
    }
}

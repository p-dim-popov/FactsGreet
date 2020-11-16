namespace FactsGreet.Web.ViewModels.Articles
{
    using System.Collections.Generic;

    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;
    using FactsGreet.Web.ViewModels.Edits;

    public class ArticleWithEditsViewModel : IMapFrom<Article>
    {
        public string Title { get; set; }

        public ICollection<EditShortDescriptionViewModel> Edits { get; set; }
    }
}

namespace FactsGreet.Web.ViewModels.Articles
{
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public class ArticleViewModel : IMapFrom<Article>
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public int FansCount { get; set; }
    }
}
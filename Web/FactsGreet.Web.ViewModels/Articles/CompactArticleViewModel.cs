namespace FactsGreet.Web.ViewModels.Articles
{
    public class CompactArticleViewModel
    {
        public string Title { get; set; }

        public string ShortContent { get; set; }

        public string[] Categories { get; set; }

        public int StarsCount { get; set; }

        public string ThumbnailLink { get; set; }
    }
}

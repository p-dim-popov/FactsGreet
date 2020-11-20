namespace FactsGreet.Services.Data.TransferObjects.Edits
{
    public class EditDto
    {
        public CompactEditDto TargetEdit { get; set; }

        public CompactEditDto AgainstEdit { get; set; }

        public string TargetArticleContent { get; set; }

        public string AgainstArticleContent { get; set; }

        public string ArticleTitle { get; set; }
    }
}

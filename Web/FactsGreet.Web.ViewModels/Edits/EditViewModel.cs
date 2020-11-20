namespace FactsGreet.Web.ViewModels.Edits
{
    using FactsGreet.Services.Data.TransferObjects.Edits;
    using FactsGreet.Services.Mapping;

    public class EditViewModel
    {
        public EditViewModel()
        { }

        public EditViewModel(EditDto editDto)
        {
            this.TargetArticleContent = editDto.TargetArticleContent;
            this.AgainstArticleContent = editDto.AgainstArticleContent;
            this.ArticleTitle = editDto.ArticleTitle;
            this.TargetEdit = new CompactEditViewModel(editDto.TargetEdit);
            this.AgainstEdit = new CompactEditViewModel(editDto.AgainstEdit);
        }

        public string TargetArticleContent { get; set; }

        public string AgainstArticleContent { get; set; }

        public string ArticleTitle { get; set; }

        public CompactEditViewModel TargetEdit { get; set; }

        public CompactEditViewModel AgainstEdit { get; set; }
    }
}

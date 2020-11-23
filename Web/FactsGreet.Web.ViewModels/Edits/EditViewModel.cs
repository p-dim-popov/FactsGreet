namespace FactsGreet.Web.ViewModels.Edits
{
    using FactsGreet.Services.Data.TransferObjects.Edits;

    public class EditViewModel
    {
        public string TargetArticleContent { get; set; }

        public string AgainstArticleContent { get; set; }

        public string ArticleTitle { get; set; }

        public CompactEditViewModel TargetEdit { get; set; }

        public CompactEditViewModel AgainstEdit { get; set; }

        public static EditViewModel CreateFrom(EditDto editDto)
        {
            return new ()
            {
                TargetArticleContent = editDto.TargetArticleContent,
                AgainstArticleContent = editDto.AgainstArticleContent,
                ArticleTitle = editDto.ArticleTitle,
                TargetEdit = CompactEditViewModel.CreateFrom(editDto.TargetEdit),
                AgainstEdit = CompactEditViewModel.CreateFrom(editDto.AgainstEdit),
            };
        }
    }
}

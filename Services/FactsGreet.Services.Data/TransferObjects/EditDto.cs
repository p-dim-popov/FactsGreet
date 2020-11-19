namespace FactsGreet.Services.Data.TransferObjects
{
    using System;

    public class EditDto
    {
        public string TargetArticleContent { get; set; }

        public string AgainstArticleContent { get; set; }

        public string ArticleTitle { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid TargetArticleId { get; set; }

        public Guid? AgainstArticleId { get; set; }
    }
}

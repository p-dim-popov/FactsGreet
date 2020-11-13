namespace FactsGreet.Web.ViewModels.Articles
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ArticleDeletionRequestCreateInputModel
    {
        [Required]
        [StringLength(450, MinimumLength = 50)]
        public string Reason { get; set; }

        public Guid Id { get; set; }

        public CompactArticleViewModel Article { get; set; }
    }
}
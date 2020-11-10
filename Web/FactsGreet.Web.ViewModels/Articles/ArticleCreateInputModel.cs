namespace FactsGreet.Web.ViewModels.Articles
{
    using System.ComponentModel.DataAnnotations;

    public class ArticleCreateInputModel
    {
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        [Required]
        [MinLength(300)]
        public string Content { get; set; }

        [MaxLength(120)]
        [MinLength(10)]
        [RegularExpression(@"^https://", ErrorMessage = "The Thumbnail link field must start with \"https://\"")]
        public string ThumbnailLink { get; set; }

        [MinLength(1)]
        public string[] Categories { get; set; }

        [MaxLength(300)]
        [MinLength(30)]
        public string Description { get; set; }
    }
}
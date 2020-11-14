using System.Linq;

namespace FactsGreet.Web.ViewModels.Articles
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using FactsGreet.Web.Infrastructure.ValidationAttributes;
    using Microsoft.AspNetCore.Http;

    public class ArticleCreateInputModel : IValidatableObject
    {
        [Required]
        [MaxLength(50)]
        [RegularExpression(
            @"^[^_%&=+].*$",
            ErrorMessage = @"The Title field should not contain illegal characters: ""_"", ""%"", ""&"", ""="", ""+""")]
        public string Title { get; set; }

        [Required]
        [MinLength(300)]
        public string Content { get; set; }

        [MaxLength(120)]
        [MinLength(10)]
        [Display(Name = "Thumbnail link")]
        [RegularExpression(
            @"^https://.*",
            ErrorMessage = "The Thumbnail link field must start with \"https://\"")]
        public string ThumbnailLink { get; set; }

        [MaxFileSize(5_000_000_000)]
        [GeneralImage]
        [Display(Name = "Thumbnail image")]
        public IFormFile ThumbnailImage { get; set; }

        [Required]
        public string[] Categories { get; set; }

        [MaxLength(300)]
        [MinLength(30)]
        public string Description { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.ThumbnailLink is { } && this.ThumbnailImage is { })
            {
                yield return new ValidationResult(
                    "Cannot supply link and upload file at the same time. Please choose only one");
            }

            if (this.Categories.All(string.IsNullOrWhiteSpace))
            {
                yield return new ValidationResult("Must enter at least one category");
            }
        }
    }
}
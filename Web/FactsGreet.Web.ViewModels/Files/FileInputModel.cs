namespace FactsGreet.Web.ViewModels.Files
{
    using System.ComponentModel.DataAnnotations;
    using FactsGreet.Web.Infrastructure.ValidationAttributes;
    using Microsoft.AspNetCore.Http;

    public class FileInputModel
    {
        public string Filename { get; set; }

        [Required]
        [MaxFileSize(500_000)]
        public IFormFile File { get; set; }
    }
}

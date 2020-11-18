namespace FactsGreet.Web.ViewModels.Files
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class FileRenameInputModel
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Filename { get; set; }
    }
}

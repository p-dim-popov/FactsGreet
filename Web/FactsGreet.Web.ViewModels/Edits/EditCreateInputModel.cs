namespace FactsGreet.Web.ViewModels.Edits
{
    using System.ComponentModel.DataAnnotations;

    using FactsGreet.Web.ViewModels.Articles;

    public class EditCreateInputModel
    {
        public ArticleCreateEditInputModel Article { get; set; }

        [Required]
        [Display(Name = "Edit summary")]
        [StringLength(450)]
        public string Comment { get; set; }
    }
}

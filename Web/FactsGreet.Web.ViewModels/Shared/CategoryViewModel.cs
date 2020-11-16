namespace FactsGreet.Web.ViewModels.Shared
{
    using System.ComponentModel.DataAnnotations;

    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public class CategoryViewModel : IMapFrom<Category>
    {
        [Required]
        [Display(Name = "Category name")]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }
    }
}

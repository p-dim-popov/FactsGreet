namespace FactsGreet.Web.ViewModels.Profiles
{
    using System.ComponentModel.DataAnnotations;

    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public class CreateAdminRequestInputModel : IMapTo<AdminRequest>
    {
        [Required]
        [StringLength(450, MinimumLength = 50)]
        [Display(Name = "Motivational letter")]
        public string MotivationalLetter { get; set; }
    }
}

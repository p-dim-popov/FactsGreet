using System.ComponentModel.DataAnnotations;

namespace FactsGreet.Data.Models
{
    using System;

    using FactsGreet.Data.Common.Models;

    public class Report : BaseDeletableModel<Guid>, IDeletableEntity, IAuditInfo
    {
        [Required]
        public string DenouncerId { get; set; }

        public ApplicationUser Denouncer { get; set; }

        [Required]
        public string RecipentId { get; set; }

        public ApplicationUser Recipent { get; set; }

        [Required]
        [MaxLength(450)]
        public string Description { get; set; }
    }
}

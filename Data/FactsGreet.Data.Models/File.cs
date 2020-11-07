using System;
using System.ComponentModel.DataAnnotations;
using FactsGreet.Data.Common.Models;

namespace FactsGreet.Data.Models
{
    public class File : BaseDeletableModel<Guid>, IDeletableEntity, IAuditInfo
    {
        [Required]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        [Required]
        [MaxLength(50)]
        public string Link { get; set; }

        [Required]
        [MaxLength(50)]
        public string Filename { get; set; }
    }
}

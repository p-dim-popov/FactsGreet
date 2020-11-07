namespace FactsGreet.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using FactsGreet.Data.Common.Models;

    public class Modification : BaseDeletableModel<Guid>, IDeletableEntity, IAuditInfo
    {
        public long Line { get; set; }

        [Required]
        public string Up { get; set; }

        [Required]
        public string Down { get; set; }
    }
}

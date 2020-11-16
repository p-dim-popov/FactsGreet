namespace FactsGreet.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using FactsGreet.Data.Common.Models;
    using FactsGreet.Data.Models.Enums;

    public class Diff : BaseDeletableModel<Guid>, IDeletableEntity, IAuditInfo
    {
        public long Index { get; set; }

        [Required]
        public string Text { get; set; }

        public DiffOperation Operation { get; set; }
    }
}

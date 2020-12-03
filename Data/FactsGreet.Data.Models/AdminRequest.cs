namespace FactsGreet.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using FactsGreet.Data.Common.Models;
    using FactsGreet.Data.Models.Enums;

    public class AdminRequest : BaseDeletableModel<Guid>, IDeletableEntity, IAuditInfo
    {
        [Required]
        [MaxLength(450)]
        public string MotivationalLetter { get; set; }

        public virtual Request Request { get; set; }
            = new Request { Type = RequestType.ArticleDeletion };
    }
}

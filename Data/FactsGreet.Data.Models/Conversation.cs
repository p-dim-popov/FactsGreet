namespace FactsGreet.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using FactsGreet.Data.Common.Models;

    public class Conversation : BaseDeletableModel<Guid>, IDeletableEntity, IAuditInfo
    {
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        public string CreatorId { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
            = new HashSet<Message>();

        public virtual ICollection<ApplicationUser> Users { get; set; }
            = new HashSet<ApplicationUser>();
    }
}

namespace FactsGreet.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using FactsGreet.Data.Common.Models;

    public class Edit : BaseDeletableModel<Guid>, IDeletableEntity, IAuditInfo
    {
        [Required]
        public string EditorId { get; set; }

        public virtual ApplicationUser Editor { get; set; }

        public Guid ArticleId { get; set; }

        public virtual Article Article { get; set; }

        public bool IsCreation { get; set; }

        public virtual ICollection<Modification> Modifications { get; set; }
            = new HashSet<Modification>();
    }
}

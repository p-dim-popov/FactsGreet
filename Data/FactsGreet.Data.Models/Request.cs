namespace FactsGreet.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using FactsGreet.Data.Common.Models;
    using FactsGreet.Data.Models.Enums;

    public class Request : BaseDeletableModel<Guid>, IDeletableEntity, IAuditInfo
    {
        public RequestType Type { get; set; }

        [Required]
        public string SenderId { get; set; }

        public virtual ApplicationUser Sender { get; set; }

        public virtual ICollection<ArticleDeletionRequest> ArticleDeletionRequests { get; set; }
            = new HashSet<ArticleDeletionRequest>();

        public virtual ICollection<AdminRequest> AdminRequests { get; set; }
            = new HashSet<AdminRequest>();
    }
}

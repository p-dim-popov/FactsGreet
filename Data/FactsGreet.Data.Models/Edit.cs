namespace FactsGreet.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using FactsGreet.Data.Common.Models;
    using FactsGreet.Data.Models.Enums;

    public class Edit : BaseDeletableModel<Guid>, IDeletableEntity, IAuditInfo
    {
        [Required]
        public string EditorId { get; set; }

        public virtual ApplicationUser Editor { get; set; }

        public Guid ArticleId { get; set; }

        public virtual Article Article { get; set; }

        public bool IsCreation { get; set; }

        [Required]
        [MaxLength(450)]
        public string Comment { get; set; }

        public Guid NotificationId { get; set; }

        public virtual Notification Notification { get; set; }
            = new Notification { Type = NotificationType.Edit };

        [Required]
        public string Patch { get; set; }
    }
}

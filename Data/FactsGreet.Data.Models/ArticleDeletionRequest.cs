namespace FactsGreet.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using FactsGreet.Data.Common.Models;
    using FactsGreet.Data.Models.Enums;

    public class ArticleDeletionRequest : BaseDeletableModel<Guid>, IDeletableEntity, IAuditInfo
    {
        public Guid ArticleId { get; set; }

        public virtual Article Article { get; set; }

        [Required]
        [MaxLength(450)]
        public string Reason { get; set; }

        public Guid NotificationId { get; set; }

        public virtual Notification Notification { get; set; }
            = new Notification{Type = NotificationType.ArticleDeleteRequest};
    }
}
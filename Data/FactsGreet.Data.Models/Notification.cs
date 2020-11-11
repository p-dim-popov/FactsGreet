namespace FactsGreet.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using FactsGreet.Data.Common.Models;
    using FactsGreet.Data.Models.Enums;

    public class Notification : BaseDeletableModel<Guid>, IDeletableEntity, IAuditInfo
    {
        public NotificationType Type { get; set; }

        [Required]
        public string SenderId { get; set; }

        public ApplicationUser Sender { get; set; }

        public virtual ICollection<ApplicationUser> Seens { get; set; }
            = new HashSet<ApplicationUser>();

        public virtual ICollection<MessageNotification> MessageNotifications { get; set; }
            = new HashSet<MessageNotification>();

        public virtual ICollection<EditNotification> EditNotifications { get; set; }
            = new HashSet<EditNotification>();

        public virtual ICollection<ArticleDeletionRequest> ArticleDeletionRequestNotifications { get; set; }
            = new HashSet<ArticleDeletionRequest>();
    }
}
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

        public virtual ICollection<Message> MessageNotifications { get; set; }
            = new HashSet<Message>();

        public virtual ICollection<Edit> EditNotifications { get; set; }
            = new HashSet<Edit>();

        public virtual ICollection<ArticleDeletionRequest> ArticleDeletionRequestNotifications { get; set; }
            = new HashSet<ArticleDeletionRequest>();
    }
}

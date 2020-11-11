namespace FactsGreet.Data.Models
{
    using System;
    using System.Collections.Generic;

    using FactsGreet.Data.Common.Models;
    using FactsGreet.Data.Models.Enums;

    public class MessageNotification : BaseDeletableModel<Guid>
    {
        public Guid MessageId { get; set; }

        public Message Message { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }
            = new HashSet<ApplicationUser>();

        public Guid NotificationId { get; set; }

        public Notification Notification { get; set; }
            = new Notification {Type = NotificationType.Message};
    }
}
namespace FactsGreet.Data.Models
{
    using System;

    using FactsGreet.Data.Common.Models;
    using FactsGreet.Data.Models.Enums;

    public class EditNotification : BaseDeletableModel<Guid>, IDeletableEntity, IAuditInfo
    {
        public Guid EditId { get; set; }

        public Edit Edit { get; set; }

        public Guid NotificationId { get; set; }

        public Notification Notification { get; set; }
            = new Notification {Type = NotificationType.Edit};
    }
}
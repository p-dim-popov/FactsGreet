namespace FactsGreet.Data.Models
{
    using FactsGreet.Data.Common.Models;
    using FactsGreet.Data.Models.Enums;

    public class Notification : BaseDeletableModel<int>, IDeletableEntity, IAuditInfo
    {
        public NotificationType Type { get; set; }

        public string SenderId { get; set; }

        public ApplicationUser Sender { get; set; }

        public string ReceiverId { get; set; }

        public ApplicationUser Receiver { get; set; }

        public bool IsSeen { get; set; }
    }
}

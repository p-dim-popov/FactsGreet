namespace FactsGreet.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using FactsGreet.Data.Common.Models;
    using FactsGreet.Data.Models.Enums;

    public class Notification : BaseDeletableModel<int>, IDeletableEntity, IAuditInfo
    {
        public NotificationType Type { get; set; }

        [Required]
        public string SenderId { get; set; }

        public ApplicationUser Sender { get; set; }

        [Required]
        public string ReceiverId { get; set; }

        public ApplicationUser Receiver { get; set; }

        public bool IsSeen { get; set; }
    }
}

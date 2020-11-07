namespace FactsGreet.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using FactsGreet.Data.Common.Models;

    public class Message : BaseDeletableModel<Guid>, IDeletableEntity, IAuditInfo
    {
        [Required]
        public string SenderId { get; set; }

        public ApplicationUser Sender { get; set; }

        public Guid ConversationId { get; set; }

        public virtual Conversation Conversation { get; set; }

        [Required]
        [MaxLength(450)]
        public string Content { get; set; }
    }
}

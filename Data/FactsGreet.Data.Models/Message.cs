namespace FactsGreet.Data.Models
{
    using System;

    using FactsGreet.Data.Common.Models;

    public class Message : BaseDeletableModel<Guid>, IDeletableEntity, IAuditInfo
    {
        public string SenderId { get; set; }

        public ApplicationUser Sender { get; set; }

        public Guid ConversationId { get; set; }

        public virtual Conversation Conversation { get; set; }

        public string Content { get; set; }
    }
}

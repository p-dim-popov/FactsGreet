namespace FactsGreet.Data.Models
{
    using System;

    public class ConversationParticipance
    {
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public Guid ConversationId { get; set; }

        public virtual Conversation Conversation { get; set; }
    }
}

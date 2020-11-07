﻿namespace FactsGreet.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using FactsGreet.Data.Common.Models;

    public class Conversation : BaseDeletableModel<Guid>, IDeletableEntity, IAuditInfo
    {
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        public string CreatorId { get; set; }

        public virtual ApplicationUser Creator { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
            = new HashSet<Message>();

        public virtual ICollection<ConversationParticipance> Participants { get; set; }
            = new HashSet<ConversationParticipance>();
    }
}
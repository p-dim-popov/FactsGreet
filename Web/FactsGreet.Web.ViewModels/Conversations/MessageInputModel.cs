﻿namespace FactsGreet.Web.ViewModels.Conversations
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Mvc;

    public class MessageInputModel
    {
        public Guid ConversationId { get; set; }

        [Required]
        [MaxLength(450)]
        public string Content { get; set; }
    }
}

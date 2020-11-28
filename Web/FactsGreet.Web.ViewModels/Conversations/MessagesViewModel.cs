namespace FactsGreet.Web.ViewModels.Conversations
{
    using System;
    using System.Collections.Generic;

    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public class MessagesViewModel : IMapFrom<Message>
    {
        public string ConversationTitle { get; set; }

        public Guid ConversationId { get; set; }
    }
}

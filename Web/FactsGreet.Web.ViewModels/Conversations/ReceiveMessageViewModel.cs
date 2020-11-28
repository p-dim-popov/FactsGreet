namespace FactsGreet.Web.ViewModels.Conversations
{
    using System;
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public class ReceiveMessageViewModel : IMapFrom<Message>
    {
        public Guid Id { get; set; }

        public Guid ConversationId { get; set; }
    }
}

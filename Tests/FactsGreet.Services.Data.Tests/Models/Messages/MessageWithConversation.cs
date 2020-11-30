namespace FactsGreet.Services.Data.Tests.Models.Messages
{
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public class MessageWithConversation : IMapFrom<Message>
    {
        public Conversation Conversation { get; set; }
    }
}

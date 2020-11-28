namespace FactsGreet.Web.ViewModels.Conversations
{
    using System;

    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public class ConversationViewModel : IMapFrom<Conversation>
    {
        public Guid Id { get; set; }

        public string Title { get; set; }
    }
}

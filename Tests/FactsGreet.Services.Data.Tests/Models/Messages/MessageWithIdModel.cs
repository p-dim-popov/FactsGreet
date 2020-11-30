namespace FactsGreet.Services.Data.Tests.Models.Messages
{
    using System;

    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public class MessageWithIdModel : IMapFrom<Message>
    {
        public Guid Id { get; set; }
    }
}

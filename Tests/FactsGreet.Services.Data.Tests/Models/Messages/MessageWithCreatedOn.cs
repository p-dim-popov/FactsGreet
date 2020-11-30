namespace FactsGreet.Services.Data.Tests.Models.Messages
{
    using System;
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public class MessageWithCreatedOn : IMapFrom<Message>
    {
        public DateTime CreatedOn { get; set; }
    }
}

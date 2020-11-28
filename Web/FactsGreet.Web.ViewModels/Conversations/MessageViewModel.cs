namespace FactsGreet.Web.ViewModels.Conversations
{
    using System;
    using System.Text.Encodings.Web;
    using System.Text.Json.Serialization;

    using AutoMapper;
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public class MessageViewModel : IMapFrom<Message>
    {
        public Guid Id { get; set; }

        public string SenderId { get; set; }

        public Guid ConversationId { get; set; }

        [JsonIgnore]
        public string Content { get; set; }

        [JsonPropertyName("content")]
        public string ContentEncoded => HtmlEncoder.Default.Encode(this.Content);

        [JsonIgnore]
        public DateTime CreatedOn { get; set; }

        [IgnoreMap]
        [JsonPropertyName("createdOn")]
        public string CreatedOnString => this.CreatedOn.ToString("R");

        public string SenderUserName { get; set; }
    }
}

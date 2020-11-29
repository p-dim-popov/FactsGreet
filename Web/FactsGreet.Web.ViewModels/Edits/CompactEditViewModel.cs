namespace FactsGreet.Web.ViewModels.Edits
{
    using System;
    using System.Text.Json.Serialization;
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Data.TransferObjects.Edits;
    using FactsGreet.Services.Mapping;

    public class CompactEditViewModel : IMapFrom<Edit>, IMapFrom<CompactEditDto>
    {
        public Guid Id { get; set; }

        public string EditorUserName { get; set; }

        [JsonIgnore]
        public DateTime CreatedOn { get; set; }

        [JsonPropertyName("createdOn")]
        public string CreatedOnString => this.CreatedOn.ToString("R");

        public string Comment { get; set; }

        public static CompactEditViewModel CreateFrom(CompactEditDto compactEditDto)
            => new CompactEditViewModel
            {
                Id = compactEditDto.Id,
                EditorUserName = compactEditDto.EditorUserName,
                CreatedOn = compactEditDto.CreatedOn,
                Comment = compactEditDto.Comment,
            };
    }
}

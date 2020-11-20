namespace FactsGreet.Web.ViewModels.Edits
{
    using System;
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Data.TransferObjects.Edits;
    using FactsGreet.Services.Mapping;

    public class CompactEditViewModel : IMapFrom<Edit>, IMapFrom<CompactEditDto>
    {
        public CompactEditViewModel()
        { }

        public CompactEditViewModel(CompactEditDto compactEditDto)
        {
            this.Id = compactEditDto.Id;
            this.EditorUserName = compactEditDto.EditorUserName;
            this.CreatedOn = compactEditDto.CreatedOn;
            this.Comment = compactEditDto.Comment;
        }

        public Guid Id { get; set; }

        public string EditorUserName { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Comment { get; set; }
    }
}

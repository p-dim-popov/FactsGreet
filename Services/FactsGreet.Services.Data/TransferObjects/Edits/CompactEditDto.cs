namespace FactsGreet.Services.Data.TransferObjects.Edits
{
    using System;

    public class CompactEditDto
    {
        public Guid Id { get; set; }

        public string EditorUserName { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Comment { get; set; }
    }
}

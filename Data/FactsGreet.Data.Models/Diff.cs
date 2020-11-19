namespace FactsGreet.Data.Models
{
    using System;

    using FactsGreet.Data.Common.Models;
    using FactsGreet.Data.Models.Enums;

    public class Diff : BaseModel<Guid>
    {
        public DiffOperation Operation { get; set; }

        public string Text { get; set; }
    }
}

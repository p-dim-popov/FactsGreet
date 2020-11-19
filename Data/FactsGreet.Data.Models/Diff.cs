namespace FactsGreet.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    using FactsGreet.Data.Common.Models;
    using FactsGreet.Data.Models.Enums;

    [Table("Diffs")]
    public class Diff : BaseModel<Guid>
    {
        public DiffOperation Operation { get; set; }

        public string Text { get; set; }
    }
}

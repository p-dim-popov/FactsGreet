namespace FactsGreet.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    using FactsGreet.Data.Common.Models;

    [Table("Patches")]
    public class Patch : BaseModel<Guid>
    {
        public int Index { get; set; }

        public int Start1 { get; set; }

        public int Start2 { get; set; }

        public int Length1 { get; set; }

        public int Length2 { get; set; }

        public virtual ICollection<Diff> Diffs { get; set; }
    }
}

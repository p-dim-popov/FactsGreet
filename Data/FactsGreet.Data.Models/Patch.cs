namespace FactsGreet.Data.Models
{
    using System;
    using System.Collections.Generic;

    using FactsGreet.Data.Common.Models;

    public class Patch : BaseModel<Guid>
    {
        public int Start1 { get; set; }

        public int Start2 { get; set; }

        public int Length1 { get; set; }

        public int Length2 { get; set; }

        public virtual ICollection<Diff> Diffs { get; set; }
    }
}

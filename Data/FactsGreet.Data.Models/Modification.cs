namespace FactsGreet.Data.Models
{
    using System;

    using FactsGreet.Data.Common.Models;

    public class Modification : BaseDeletableModel<Guid>, IDeletableEntity, IAuditInfo
    {
        public long Line { get; set; }

        public string Up { get; set; }

        public string Down { get; set; }
    }
}

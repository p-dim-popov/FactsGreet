namespace FactsGreet.Data.Models
{
    using System;

    using FactsGreet.Data.Common.Models;

    public class Report : BaseDeletableModel<Guid>, IDeletableEntity, IAuditInfo
    {
        public string DenouncerId { get; set; }

        public ApplicationUser Denouncer { get; set; }

        public string RecipentId { get; set; }

        public ApplicationUser Recipent { get; set; }

        public string Description { get; set; }
    }
}

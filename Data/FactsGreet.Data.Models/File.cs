using System;
using FactsGreet.Data.Common.Models;

namespace FactsGreet.Data.Models
{
    public class File : BaseDeletableModel<Guid>, IDeletableEntity, IAuditInfo
    {
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public string Link { get; set; }

        public string Filename { get; set; }
    }
}

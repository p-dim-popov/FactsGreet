namespace FactsGreet.Data.Models
{
    using System;
    using System.Collections.Generic;

    using FactsGreet.Data.Common.Models;

    public class Badge : BaseModel<Guid>, IAuditInfo
    {
        public string Name { get; set; }

        public virtual ICollection<ApplicationUser> UsersWithBadges { get; set; }
            = new HashSet<ApplicationUser>();
    }
}
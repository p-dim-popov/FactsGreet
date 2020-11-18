namespace FactsGreet.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using FactsGreet.Data.Common.Models;

    public class Star : BaseModel<int>
    {
        public Guid ArticleId { get; set; }

        public virtual Article Article { get; set; }

        [Required]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}

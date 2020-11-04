namespace FactsGreet.Data.Models
{
    using System;

    public class Star
    {
        public Guid ArticleId { get; set; }

        public virtual Article Article { get; set; }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}

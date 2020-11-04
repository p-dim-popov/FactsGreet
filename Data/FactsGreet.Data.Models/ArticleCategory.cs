namespace FactsGreet.Data.Models
{
    using System;

    public class ArticleCategory
    {
        public Guid ArticleId { get; set; }

        public Article Article { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }
    }
}

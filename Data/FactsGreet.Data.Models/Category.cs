namespace FactsGreet.Data.Models
{
    using System.Collections.Generic;

    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<ArticleCategory> Articles { get; set; }
            = new HashSet<ArticleCategory>();
    }
}

namespace FactsGreet.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using FactsGreet.Data.Common.Models;

    public class Category : BaseModel<int>, IAuditInfo
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public virtual ICollection<Article> Articles { get; set; }
            = new HashSet<Article>();
    }
}

namespace FactsGreet.Data.Models
{
    using System;
    using System.Collections.Generic;

    using FactsGreet.Data.Common.Models;

    public class Article : BaseDeletableModel<Guid>, IDeletableEntity, IAuditInfo
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public string ThumbnailLink { get; set; }

        public virtual ICollection<ArticleCategory> Categories { get; set; }
            = new HashSet<ArticleCategory>();

        public virtual ICollection<Edit> Edits { get; set; }
            = new HashSet<Edit>();

        public virtual ICollection<Star> Fans { get; set; }
            = new HashSet<Star>();
    }
}

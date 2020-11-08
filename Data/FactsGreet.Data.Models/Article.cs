namespace FactsGreet.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using FactsGreet.Data.Common.Models;

    public class Article : BaseDeletableModel<Guid>, IDeletableEntity, IAuditInfo
    {
        [Required] [MaxLength(50)] public string Title { get; set; }

        [Required] public string Content { get; set; }

        [MaxLength(300)] public string Description { get; set; }

        [MaxLength(120)]
        public string ThumbnailLink { get; set; }
            = "https://upload.wikimedia.org/wikipedia/commons/thumb/a/ac/No_image_available.svg/200px-No_image_available.svg.png";

        [Required] public string AuthorId { get; set; }

        public virtual ApplicationUser Author { get; set; }

        public virtual ICollection<ArticleCategory> Categories { get; set; }
            = new HashSet<ArticleCategory>();

        public virtual ICollection<Edit> Edits { get; set; }
            = new HashSet<Edit>();

        public virtual ICollection<Star> Stars { get; set; }
            = new HashSet<Star>();
    }
}
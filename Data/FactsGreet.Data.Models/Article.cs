namespace FactsGreet.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using FactsGreet.Data.Common.Models;

    public class Article : BaseDeletableModel<Guid>, IDeletableEntity, IAuditInfo
    {
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        [MaxLength(120)]
        public string ThumbnailLink { get; set; }

        [Required]
        public string AuthorId { get; set; }

        public virtual ApplicationUser Author { get; set; }

        public virtual ICollection<Category> Categories { get; set; }
            = new HashSet<Category>();

        public virtual ICollection<Edit> Edits { get; set; }
            = new HashSet<Edit>();

        public virtual ICollection<Star> Stars { get; set; }
            = new HashSet<Star>();

        public virtual ICollection<ArticleDeletionRequest> DeletionRequests { get; set; }
            = new HashSet<ArticleDeletionRequest>();
    }
}

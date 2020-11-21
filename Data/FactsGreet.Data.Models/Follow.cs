namespace FactsGreet.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using FactsGreet.Data.Common.Models;

    public class Follow : BaseDeletableModel<int>
    {
        [Required]
        public string FollowerId { get; set; }

        public virtual ApplicationUser Follower { get; set; }

        [Required]
        public string FollowedId { get; set; }

        public virtual ApplicationUser Followed { get; set; }
    }
}

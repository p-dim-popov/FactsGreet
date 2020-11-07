namespace FactsGreet.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Follow
    {
        public string FollowerId { get; set; }

        public virtual ApplicationUser Follower { get; set; }

        public string FollowedId { get; set; }

        public virtual ApplicationUser Followed { get; set; }
    }
}

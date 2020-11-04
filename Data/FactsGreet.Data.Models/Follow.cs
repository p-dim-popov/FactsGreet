namespace FactsGreet.Data.Models
{
    public class Follow
    {
        public string FollowerId { get; set; }

        public virtual ApplicationUser Follower { get; set; }

        public string FollowedId { get; set; }

        public virtual ApplicationUser Followed { get; set; }
    }
}

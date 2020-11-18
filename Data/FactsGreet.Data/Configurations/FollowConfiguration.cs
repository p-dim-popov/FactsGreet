namespace FactsGreet.Data.Configurations
{
    using FactsGreet.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class FollowConfiguration : IEntityTypeConfiguration<Follow>
    {
        public void Configure(EntityTypeBuilder<Follow> follow)
        {
            follow
                .HasIndex(x => new { x.FollowerId, x.FollowedId })
                .IsUnique();

            follow
                .HasOne(x => x.Follower)
                .WithMany(x => x.Followings)
                .HasForeignKey(x => x.FollowerId);

            follow
                .HasOne(x => x.Followed)
                .WithMany(x => x.Followers)
                .HasForeignKey(x => x.FollowedId);
        }
    }
}

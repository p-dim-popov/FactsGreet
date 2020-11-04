namespace FactsGreet.Data.Configurations
{
    using FactsGreet.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class FollowConfiguration : IEntityTypeConfiguration<Follow>
    {
        public void Configure(EntityTypeBuilder<Follow> builder)
        {
            builder.HasKey(x => new { x.FollowerId, x.FollowedId });

            builder
                .HasOne(x => x.Follower)
                .WithMany(x => x.Followings)
                .HasForeignKey(x => x.FollowerId);

            builder
                .HasOne(x => x.Followed)
                .WithMany(x => x.Followers)
                .HasForeignKey(x => x.FollowedId);
        }
    }
}

namespace FactsGreet.Data.Configurations
{
    using FactsGreet.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class StarConfiguration : IEntityTypeConfiguration<Star>
    {
        public void Configure(EntityTypeBuilder<Star> star)
        {
            star
                .HasIndex(x => new { x.UserId, x.ArticleId })
                .IsUnique();

            star
                .HasOne(x => x.Article)
                .WithMany(x => x.Stars)
                .HasForeignKey(x => x.ArticleId);

            star
                .HasOne(x => x.User)
                .WithMany(x => x.StarredArticles)
                .HasForeignKey(x => x.UserId);
        }
    }
}

namespace FactsGreet.Data.Configurations
{
    using FactsGreet.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class StarConfiguration : IEntityTypeConfiguration<Star>
    {
        public void Configure(EntityTypeBuilder<Star> builder)
        {
            builder.HasKey(x => new { x.UserId, x.ArticleId });

            builder
                .HasOne(x => x.User)
                .WithMany(x => x.FavoriteArticles)
                .HasForeignKey(x => x.UserId);

            builder
                .HasOne(x => x.Article)
                .WithMany(x => x.Stars)
                .HasForeignKey(x => x.ArticleId);
        }
    }
}

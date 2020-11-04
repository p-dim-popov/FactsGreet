namespace FactsGreet.Data.Configurations
{
    using FactsGreet.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ArticleCategoryConfiguration : IEntityTypeConfiguration<ArticleCategory>
    {
        public void Configure(EntityTypeBuilder<ArticleCategory> builder)
        {
            builder.HasKey(x => new { x.ArticleId, x.CategoryId });

            builder
                .HasOne(x => x.Article)
                .WithMany(x => x.Categories)
                .HasForeignKey(x => x.ArticleId);

            builder
                .HasOne(x => x.Category)
                .WithMany(x => x.Articles)
                .HasForeignKey(x => x.CategoryId);
        }
    }
}

namespace FactsGreet.Data.Configurations
{
    using FactsGreet.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ArticleDeletionRequestConfiguration : IEntityTypeConfiguration<ArticleDeletionRequest>
    {
        public void Configure(EntityTypeBuilder<ArticleDeletionRequest> articleDeletionRequest)
        {
            articleDeletionRequest
                .HasOne(x => x.Article)
                .WithOne(x => x.DeletionRequest)
                .HasForeignKey<ArticleDeletionRequest>(x => x.ArticleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

namespace FactsGreet.Data.Configurations
{
    using FactsGreet.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class RequestConfiguration : IEntityTypeConfiguration<Request>
    {
        public void Configure(EntityTypeBuilder<Request> request)
        {
            request
                .HasOne(x => x.Sender)
                .WithMany(x => x.Requests)
                .HasForeignKey(x => x.SenderId);

            request
                .HasMany(x => x.ArticleDeletionRequests)
                .WithOne(x => x.Request)
                .OnDelete(DeleteBehavior.Cascade);

            request
                .HasMany(x => x.AdminRequests)
                .WithOne(x => x.Request)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

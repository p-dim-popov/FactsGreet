namespace FactsGreet.Data.Configurations
{
    using FactsGreet.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    // public class RequestConfiguration : IEntityTypeConfiguration<ArticleDeletionRequest>
    // {
    //     public void Configure(EntityTypeBuilder<ArticleDeletionRequest> builder)
    //     {
    //         builder
    //             .HasOne(x => x.Requester)
    //             .WithMany(x => x.SentRequests)
    //             .HasForeignKey(x => x.RequesterId);
    //
    //         builder
    //             .HasOne(x => x.Accused)
    //             .WithMany(x => x.ReceivedRequests)
    //             .HasForeignKey(x => x.AccusedId);
    //     }
    // }
}

namespace FactsGreet.Data.Configurations
{
    using FactsGreet.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ReportConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder
                .HasOne(x => x.Denouncer)
                .WithMany(x => x.SentReports)
                .HasForeignKey(x => x.DenouncerId);

            builder
                .HasOne(x => x.Recipent)
                .WithMany(x => x.ReceivedReports)
                .HasForeignKey(x => x.RecipentId);
        }
    }
}

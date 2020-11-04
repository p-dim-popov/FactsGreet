namespace FactsGreet.Data.Configurations
{
    using FactsGreet.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder
                .HasOne(x => x.Sender)
                .WithMany(x => x.Actions)
                .HasForeignKey(x => x.SenderId);

            builder
                .HasOne(x => x.Receiver)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.ReceiverId);
        }
    }
}

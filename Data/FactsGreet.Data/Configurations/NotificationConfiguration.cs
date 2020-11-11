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
                .HasMany(x => x.Seens)
                .WithMany(x => x.SeenNotifications);

            builder
                .HasOne(x => x.Sender)
                .WithMany(x => x.Notifications);
        }
    }
}
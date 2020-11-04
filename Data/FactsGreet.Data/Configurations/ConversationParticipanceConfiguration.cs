namespace FactsGreet.Data.Configurations
{
    using FactsGreet.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ConversationParticipanceConfiguration : IEntityTypeConfiguration<ConversationParticipance>
    {
        public void Configure(EntityTypeBuilder<ConversationParticipance> builder)
        {
            builder.HasKey(x => new { x.ConversationId, x.UserId });

            builder
                .HasOne(x => x.User)
                .WithMany(x => x.ConversationParticipances)
                .HasForeignKey(x => x.UserId);

            builder
                .HasOne(x => x.Conversation)
                .WithMany(x => x.Participants)
                .HasForeignKey(x => x.ConversationId);
        }
    }
}

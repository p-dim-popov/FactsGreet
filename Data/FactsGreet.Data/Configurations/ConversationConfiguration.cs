namespace FactsGreet.Data.Configurations
{
    using FactsGreet.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
    {
        public void Configure(EntityTypeBuilder<Conversation> builder)
        {
            builder
                .HasMany(x => x.Users)
                .WithMany(x => x.Conversations);

            builder
                .HasOne(x => x.Creator)
                .WithMany(x => x.CreatedConversations);
        }
    }
}
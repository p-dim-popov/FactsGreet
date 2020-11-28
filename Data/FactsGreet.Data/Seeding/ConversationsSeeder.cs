namespace FactsGreet.Data.Seeding
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FactsGreet.Common;
    using FactsGreet.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class ConversationsSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (await dbContext.Conversations.AnyAsync())
            {
                return;
            }

            var users = await dbContext.Users
                .OrderBy(x => x.CreatedOn)
                .Take(2)
                .ToListAsync();

            var conversation = new Conversation
            {
                Title = users.Select(x => x.UserName).Join(", "),
                Users = users,
                Messages =
                {
                    new Message
                    {
                        Content = "1) hi!",
                        Sender = users.First(),
                        CreatedOn = DateTime.UtcNow.AddMinutes(1),
                    },
                    new Message
                    {
                        Content = "2) oh, hello there!",
                        Sender = users.Last(),
                        CreatedOn = DateTime.UtcNow.AddMinutes(2),
                    },
                    new Message
                    {
                        Content = "3) how are you??",
                        Sender = users.Last(),
                        CreatedOn = DateTime.UtcNow.AddMinutes(3),
                    },
                    new Message
                    {
                        Content = "4) gud.",
                        Sender = users.First(),
                        CreatedOn = DateTime.UtcNow.AddMinutes(4),
                    },
                    new Message
                    {
                        Content = "5) Cat ipsum dolor sit amet, magni occaecat. " +
                                  "Iure ipsa, dolore exercitation voluptas. " +
                                  "Mollit do consequatur yet dolores ab or enim for est. " +
                                  "Numquam. Omnis. Nostrud veritatis, magnam so voluptas yet corporis esse. " +
                                  "Cupidatat laboriosam, mollit aliqua so consequuntur. ",
                        Sender = users.Last(),
                        CreatedOn = DateTime.UtcNow.AddMinutes(5),
                    },
                },
            };

            await dbContext.Conversations.AddAsync(conversation);
            await dbContext.SaveChangesAsync();
        }
    }
}

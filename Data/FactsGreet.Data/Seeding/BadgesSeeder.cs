namespace FactsGreet.Data.Seeding
{
    using System;
    using System.Threading.Tasks;

    using FactsGreet.Common;
    using FactsGreet.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class BadgesSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (await dbContext.Badges.AnyAsync())
            {
                return;
            }

            var badges = new[]
            {
                new Badge { Name = GlobalConstants.Badges.Creator },
                new Badge { Name = GlobalConstants.Badges.Editor },
            };

            await dbContext.Badges.AddRangeAsync(badges);
            await dbContext.SaveChangesAsync();
        }
    }
}

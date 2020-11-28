namespace FactsGreet.Data.Seeding
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using FactsGreet.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class FollowsSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (await dbContext.Follows.AnyAsync())
            {
                return;
            }

            var admin = await dbContext.Users
                .Where(x => x.NormalizedEmail == "ADMIN@LOCALHOST")
                .FirstOrDefaultAsync();

            var notadmin = await dbContext.Users
                .Where(x => x.NormalizedEmail == "NOTADMIN@LOCALHOST")
                .FirstOrDefaultAsync();

            var helper = await dbContext.Users
                .Where(x => x.NormalizedEmail == "HELPER@LOCALHOST")
                .FirstOrDefaultAsync();

            admin.Followers.Add(new Follow { Follower = notadmin });
            admin.Followers.Add(new Follow { Follower = helper });
            admin.Followings.Add(new Follow { Followed = notadmin });
        }
    }
}

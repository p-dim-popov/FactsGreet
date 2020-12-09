namespace FactsGreet.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FactsGreet.Common;
    using FactsGreet.Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class UsersSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (await dbContext.Users.AnyAsync())
            {
                return;
            }

            var userManager = serviceProvider
                .GetService<UserManager<ApplicationUser>>();

            var dbBadges = await dbContext.Badges.ToListAsync();

            var configuration = serviceProvider.GetService<IConfiguration>();
            var password = configuration?.GetSection("BuiltInProfilesPassword").Value;

            var users = new (ApplicationUser User, string Role)[]
                {
                    (new ApplicationUser
                        {
                            UserName = "admin@localhost",
                            Email = "admin@localhost",
                        },
                        GlobalConstants.AdministratorRoleName),
                    (new ApplicationUser
                        {
                            UserName = "notadmin@localhost",
                            Email = "notadmin@localhost",
                        },
                        GlobalConstants.RegularRoleName),
                    (new ApplicationUser
                        {
                            UserName = "helper@localhost",
                            Email = "helper@localhost",
                        },
                        GlobalConstants.RegularRoleName),
                }
                .Concat(Enumerable.Range(1, 10)
                    .Select(x =>
                        (User: new ApplicationUser
                            {
                                UserName = $"air{x}@localhost",
                                Email = $"air{x}@localhost",
                            },
                            Role: GlobalConstants.RegularRoleName)))
                .Select(x =>
                {
                    var skip = ISeeder.Random.Next(0, dbBadges.Count);
                    var take = ISeeder.Random.Next(skip, dbBadges.Count);
                    return (x, Badges: dbBadges.Skip(skip).Take(take).ToList());
                });

            foreach (var ((user, role), badges) in users)
            {
                await SeedUserAsync(userManager, user, password, role, badges);
            }
        }

        private static async Task SeedUserAsync(
            UserManager<ApplicationUser> userManager,
            ApplicationUser user,
            string password,
            string role,
            ICollection<Badge> badges)
        {
            user.Badges = badges;

            var result = await userManager
                .CreateAsync(user, password);

            if (!result.Succeeded)
            {
                Console.WriteLine(result);
            }

            await userManager.AddToRoleAsync(user, role);
        }
    }
}

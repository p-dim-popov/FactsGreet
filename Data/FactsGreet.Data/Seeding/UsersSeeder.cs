namespace FactsGreet.Data.Seeding
{
    using System;
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

            var configuration = serviceProvider.GetService<IConfiguration>();
            var password = configuration?.GetSection("BuiltInProfilesPassword").Value;

            var users = new (ApplicationUser User, string Role, string[] Badges)[]
            {
                (new ApplicationUser
                    {
                        UserName = "admin@localhost",
                        Email = "admin@localhost",
                    },
                    GlobalConstants.AdministratorRoleName, null),
                (new ApplicationUser
                    {
                        UserName = "notadmin@localhost",
                        Email = "notadmin@localhost",
                    },
                    GlobalConstants.RegularRoleName,
                    new[] { GlobalConstants.Badges.BeingCorrect, GlobalConstants.Badges.Creator }),
                (new ApplicationUser
                    {
                        UserName = "helper@localhost",
                        Email = "helper@localhost",
                    },
                    GlobalConstants.RegularRoleName, null),
            };

            foreach (var (user, role, badges) in users)
            {
                await SeedUserAsync(dbContext, userManager, user, password, role, badges);
            }
        }

        private static async Task SeedUserAsync(
            ApplicationDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            ApplicationUser user,
            string password,
            string role,
            string[] badges)
        {
            user.Badges = badges
                ?.Select(x => new Badge { Name = x })
                .ToList();

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

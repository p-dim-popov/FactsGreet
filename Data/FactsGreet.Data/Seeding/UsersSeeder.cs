namespace FactsGreet.Data.Seeding
{
    using System;
    using System.Threading.Tasks;
    using FactsGreet.Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
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

            var admin = new ApplicationUser
            {
                UserName = "admin@localhost",
                Email = "admin@localhost",
            };

            Console.WriteLine("Enter admin's password: ");
            var result = await userManager
                .CreateAsync(admin, Console.ReadLine());

            if (!result.Succeeded)
            {
                Console.WriteLine(result);
            }

            await userManager
                .AddToRoleAsync(admin, "Administrator");
        }
    }
}
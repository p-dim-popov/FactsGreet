namespace FactsGreet.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using FactsGreet.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class CategoriesSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (await dbContext.Categories.AnyAsync())
            {
                return;
            }

            var categories = new List<string>
                    {"it", "programming", "web", "internet"}
                .Select(x => new Category {Name = x})
                .ToList();

            await dbContext.Categories.AddRangeAsync(categories);
            await dbContext.SaveChangesAsync();
        }
    }
}
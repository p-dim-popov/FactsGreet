using System.Text.RegularExpressions;

namespace FactsGreet.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FactsGreet.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;

    public class ArticlesSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (await dbContext.Articles.AnyAsync())
            {
                return;
            }

            var rng = new Random(DateTime.Now.Millisecond);
            var categories = await dbContext.Categories.ToListAsync();

            var adminId = await dbContext.Users
                .Where(x => x.UserName == "admin@localhost")
                .Select(x => x.Id)
                .FirstAsync();

            var articles = JsonConvert
                .DeserializeObject<Dictionary<string, string>[]>(await System.IO.File
                    .ReadAllTextAsync("SeedingResources/articles.json"))
                .Select(x => new Article
                {
                    AuthorId = adminId,
                    Categories = Enumerable.Range(rng.Next(), rng.Next(0, categories.Count))
                        .Select(y => new ArticleCategory {Category = categories[y % categories.Count]})
                        .ToList(),
                    Content = x["content"].Replace("/wiki/", "/Articles/"),
                    Description = x["description"],
                    Title = x["title"],
                    ThumbnailLink = x["thumbnailLink"],
                    Edits = new List<Edit>
                    {
                        new Edit
                        {
                            EditorId = adminId,
                            Modifications = new List<Modification>
                            {
                                new Modification
                                {
                                    Line = 0,
                                    Up = x["content"],
                                    Down = string.Empty,
                                },
                            },
                            IsCreation = true,
                        },
                    },
                });

            await dbContext.Articles.AddRangeAsync(articles);
            await dbContext.SaveChangesAsync();
        }
    }
}
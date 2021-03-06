﻿namespace FactsGreet.Data.Seeding
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using AngleSharp;
    using AngleSharp.Dom;
    using AngleSharp.Html.Parser;
    using FactsGreet.Data.Models;
    using FactsGreet.Services;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public class ArticlesSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (await dbContext.Articles.AnyAsync())
            {
                return;
            }

            var dmpService = serviceProvider.GetService<IDiffMatchPatchService>();
            var categories = await dbContext.Categories.ToListAsync();

            var users = await dbContext.Users
                .Select(x => x.Id)
                .ToArrayAsync();
            var editRegex = new Regex(@"\(\/w\/index.php\?.*title=(?<title>.*)&.*action=edit.*\)");

            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);

            var elementsToRemove = string.Join(
                ", ",
                "script",
                "style",
                ".infobox");

            var parentRemoval = string.Join(
                ", ",
                "#See_also",
                "#References",
                "#Further_reading",
                "#External_links",
                "#Източници",
                "#Външни_препратки");

            var markdownConverter = new Html2Markdown.Converter();
            var htmlParser = new HtmlParser();
            var articles = (await Task.WhenAll(
                    (await Task.WhenAll(new[]
                        {
                            "https://en.wikipedia.org/wiki/Microsoft",
                            "https://en.wikipedia.org/wiki/SignalR",
                            "https://en.wikipedia.org/wiki/Event-driven_programming",
                            "https://en.wikipedia.org/wiki/Computer_network_programming",
                            "https://en.wikipedia.org/wiki/.NET_Framework",
                            "https://en.wikipedia.org/wiki/Microsoft_Windows",
                            "https://en.wikipedia.org/wiki/MS-DOS",
                            "https://en.wikipedia.org/wiki/Programming_language",
                            "https://en.wikipedia.org/wiki/International_Organization_for_Standardization",
                            "https://en.wikipedia.org/wiki/Graphical_user_interface",
                            "https://en.wikipedia.org/wiki/Icon_(computing)",
                            "https://en.wikipedia.org/wiki/Text-based_user_interface",
                            "https://en.wikipedia.org/wiki/Character_(computing)",
                            "https://en.wikipedia.org/wiki/Carriage_return",
                            "https://en.wikipedia.org/wiki/Apple_Keyboard",
                            "https://bg.wikipedia.org/wiki/%D0%9A%D0%BB%D0%B0%D0%B2%D0%B8%D0%B0%D1%82%D1%83%D1%80%D0%BD%D0%B0_%D0%BF%D0%BE%D0%B4%D1%80%D0%B5%D0%B4%D0%B1%D0%B0",
                            "https://bg.wikipedia.org/wiki/%D0%A2%D0%B5%D1%84%D0%BB%D0%BE%D0%BD",
                            "https://bg.wikipedia.org/wiki/%D0%9A%D0%BE%D0%BD%D1%86%D0%B5%D1%80%D0%BD",
                            "https://bg.wikipedia.org/wiki/%D0%9A%D0%BE%D0%BD%D0%B3%D0%BB%D0%BE%D0%BC%D0%B5%D1%80%D0%B0%D1%82_(%D0%B8%D0%BA%D0%BE%D0%BD%D0%BE%D0%BC%D0%B8%D0%BA%D0%B0)",
                            "https://bg.wikipedia.org/wiki/%D0%9F%D1%80%D0%B5%D0%B4%D0%BF%D1%80%D0%B8%D1%8F%D1%82%D0%B8%D0%B5",
                            "https://en.wikipedia.org/wiki/Linux",
                            "https://en.wikipedia.org/wiki/Unix",
                            "https://en.wikipedia.org/wiki/Linus_Torvalds",
                            "https://en.wikipedia.org/wiki/Linux_kernel",
                            "https://en.wikipedia.org/wiki/Windowing_system",
                            "https://en.wikipedia.org/wiki/X86",
                            "https://en.wikipedia.org/wiki/GNU",
                            "https://en.wikipedia.org/wiki/Richard_Stallman",
                            "https://en.wikipedia.org/wiki/Emacs",
                            "https://en.wikipedia.org/wiki/GNU_Compiler_Collection",
                            "https://en.wikipedia.org/wiki/C%2B%2B17",
                            "https://en.wikipedia.org/wiki/CXX",
                            "https://en.wikipedia.org/wiki/C_(programming_language)",
                        }
                        .OrderBy(x => ISeeder.Random.Next())
                        .Select(x => context.OpenAsync(x))))
                    .Select(x =>
                    {
                        foreach (var element in x.QuerySelectorAll(elementsToRemove))
                        {
                            element.Remove();
                        }

                        var breakingPoint = x.QuerySelector(parentRemoval)?.ParentElement;
                        IElement next;
                        while ((next = breakingPoint?.NextElementSibling) is { })
                        {
                            breakingPoint.Remove();
                            breakingPoint = next;
                        }

                        var content = htmlParser.ParseDocumentAsync(
                            markdownConverter.Convert(
                                x.GetElementById("mw-content-text").InnerHtml));

                        return Task.Run(async () => new
                        {
                            Content = await content,

                            // because of some error with Split... Won't accept "-" in "Заглавие - Уикипедия"
                            // to be the same as in "Title - Wikipedia"... Too bad.
                            Title = x.Title.Substring(0, x.Title.Length - 12),
                        });
                    })))
                .Select(x => new
                {
                    Content = x.Content.Body.Text(),
                    x.Title,
                })
                .Select(x => new Article
                {
                    AuthorId = users[ISeeder.Random.Next(0, users.Length)],
                    Categories = Enumerable.Range(ISeeder.Random.Next(), ISeeder.Random.Next(0, categories.Count))
                        .Select(y => categories[y % categories.Count])
                        .ToList(),
                    Content = editRegex
                        .Replace(
                            x.Content
                                .Replace("/wiki/", "/Article/"),
                            "(/Edits/Create/${title})"),
                    Title = x.Title,
                    ThumbnailLink = $"https://picsum.photos/{ISeeder.Random.Next(200, 400)}",
                })
                .Select(x =>
                {
                    x.Edits.Add(new Edit
                    {
                        EditorId = x.AuthorId,
                        IsCreation = true,
                        Comment = "Initial create",
                        Patches = dmpService?.CreateEdit(string.Empty, x.Content),
                        CreatedOn = DateTime.UtcNow.AddDays(-ISeeder.Random.Next(0, 14)), // Two weeks range for creation because of ranking system
                    });
                    return x;
                })
                .ToList();

            await dbContext.Articles.AddRangeAsync(articles);
            await dbContext.SaveChangesAsync();
        }
    }
}

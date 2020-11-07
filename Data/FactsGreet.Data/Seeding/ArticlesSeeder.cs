namespace FactsGreet.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FactsGreet.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class ArticlesSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (await dbContext.Articles.AnyAsync())
            {
                return;
            }

            var categories = await dbContext.Categories.ToListAsync();

            var adminId = await dbContext.Users
                .Where(x => x.UserName == "admin@localhost")
                .Select(x => x.Id)
                .FirstAsync();

            var articles =
                new List<(
                    string AuthorId,
                    string Title,
                    string Content,
                    Category[] Categories,
                    string ThumbnailLink,
                    string Description)>
                {
                    (adminId,
                        ".NET Core",
                        "**.NET Core** is a [free and open-source](https://en.wikipedia.org/wiki/Free_and_open-source), " +
                        "managed computer software framework for [Windows](https://en.wikipedia.org/wiki/Microsoft_Windows), " +
                        "[Linux](https://en.wikipedia.org/wiki/Linux), and [macOS](https://en.wikipedia.org/wiki/MacOS)" +
                        " operating systems. It is a cross-platform successor to " +
                        "[.NET Framework](https://en.wikipedia.org/wiki/.NET_Framework). The project is primarily " +
                        "developed by Microsoft employees by way of the " +
                        ".NET Foundation, and released under the [MIT License](https://en.wikipedia.org/wiki/MIT_License).",
                        new[] {categories[0], categories[1]},
                        "https://picsum.photos/200",
                        ".NET Core is a free and open-source managed computer software framework."),
                    (adminId,
                        "ASP.NET Core",
                        "**ASP.NET Core** is a free and open-source web framework and successor " +
                        "to ASP.NET, developed by Microsoft. It is a modular " +
                        "framework that runs on both the full [.NET Framework](https://en.wikipedia.org/wiki/.NET_Framework), " +
                        "on Windows, and the cross-platform .NET Core. However " +
                        "ASP.NET Core version 3 works only on .NET Core " +
                        "dropping support of .NET Framework.",
                        new[] {categories[1], categories[2], categories[3]},
                        "https://picsum.photos/100/150",
                        "ASP.NET Core is a free and open-source web framework and successor to ASP.NET, developed by Microsoft."),
                    (adminId,
                        "Internet",
                        @"**The Internet** (or internet) is the global system of interconnected computer networks that uses the Internet protocol suite (TCP/IP) to communicate between networks and devices. It is a network of networks that consists of private, public, academic, business, and government networks of local to global scope, linked by a broad array of electronic, wireless, and optical networking technologies. The Internet carries a vast range of information resources and services, such as the inter-linked hypertext documents and applications of the World Wide Web (WWW), electronic mail, telephony, and file sharing.
The origins of the Internet date back to the development of packet switching and research commissioned by the United States Department of Defense in the 1960s to enable time-sharing of computers.",
                        new[] {categories[0], categories[1], categories[2], categories[3]},
                        string.Empty,
                        string.Empty),
                    (adminId,
                        "IP address",
                        @"From Wikipedia, the free encyclopedia
Jump to navigation
Jump to search
For the Wikipedia user access level, see Wikipedia:User access levels § Unregistered (IP or not logged in) users.

An Internet Protocol address (IP address) is a numerical label assigned to each device connected to a computer network that uses the Internet Protocol for communication.[1][2] An IP address serves two main functions: host or network interface identification and location addressing.

Internet Protocol version 4 (IPv4) defines an IP address as a 32-bit number.[2] However, because of the growth of the Internet and the depletion of available IPv4 addresses, a new version of IP (IPv6), using 128 bits for the IP address, was standardized in 1998.[3][4][5] IPv6 deployment has been ongoing since the mid-2000s. ",
                        new[] {categories[1]},
                        string.Empty,
                        "An Internet Protocol address (IP address) is a numerical label assigned to each device connected to a computer network"),
                    (adminId,
                        "Regional Internet registry",
                        @"A regional Internet registry (RIR) is an organization that manages the allocation and registration of Internet number resources within a region of the world. Internet number resources include IP addresses and autonomous system (AS) numbers. ",
                        new[] {categories[3], categories[1], categories[2]},
                        "https://picsum.photos/100/151",
                        null),
                }.Select(x => new Article
                {
                    AuthorId = adminId,
                    Categories = x.Categories.Select(y => new ArticleCategory {Category = y}).ToList(),
                    Content = x.Content,
                    Description = x.Description,
                    Title = x.Title,
                    ThumbnailLink = x.ThumbnailLink,
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
                                    Up = x.Content,
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
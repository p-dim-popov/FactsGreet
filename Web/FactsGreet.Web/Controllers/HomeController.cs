namespace FactsGreet.Web.Controllers
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    using FactsGreet.Web.ViewModels;
    using FactsGreet.Web.ViewModels.Articles;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            return this.View();
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }

        public IActionResult Feed()
        {
            return this.View(Enumerable.Range(0, 10)
                    .Select(x => new CompactArticleViewModelExtended
                    {
                        Categories = new[] { "Internet", "Programming", "WOW" },
                        ShortContent = "Veeeeeeeeeery short content for this article...",
                        StarsCount = x,
                        ThumbnailLink = "https://picsum.photos/" + new Random().Next(1024),
                        Title = "Generic title \"притежаващ\" български",
                        IsCreated = x % 2 == 0,
                        Author = "Admin Admin",
                    })
                    .ToArray());
        }
    }
}

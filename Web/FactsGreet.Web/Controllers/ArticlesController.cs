namespace FactsGreet.Web.Controllers
{
    using System;
    using System.Linq;

    using FactsGreet.Web.ViewModels.Articles;
    using Microsoft.AspNetCore.Mvc;

    public class ArticlesController : BaseController
    {
        public IActionResult GetBySlug(string slug)
        {
            if (slug is null)
            {
                return this.RedirectToAction(nameof(this.All));
            }

            return this.View("Article");
        }

        public IActionResult Search(string slug)
        {
            if (slug is null)
            {
                return this.RedirectToAction(nameof(this.All));
            }

            this.ViewBag.Slug = slug;
            return this.View(Enumerable.Range(0, 10)
                .Select(x => new CompactArticleViewModel
                {
                    Categories = new[] { "Internet", "Programming", "WOW" },
                    ShortContent = "Veeeeeeeeeery short content for this article...",
                    StarsCount = x,
                    ThumbnailLink = "https://picsum.photos/" + new Random().Next(1024),
                    Title = "Generic title с български",
                })
                .ToArray());
        }

        public IActionResult All()
        {
            return this.View(Enumerable.Range(0, 10)
                .Select(x => new CompactArticleViewModel
                {
                    Categories = new[] { "Internet", "Programming", "WOW", "One more" },
                    ShortContent = "Veeeeeeeeeery short content for this article...",
                    StarsCount = x,
                    ThumbnailLink = "https://picsum.photos/" + new Random().Next(1024),
                    Title = "Generic title с български",
                })
                .ToArray());
        }
    }
}

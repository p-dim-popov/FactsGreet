namespace FactsGreet.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FactsGreet.Common;
    using FactsGreet.Services.Data;
    using FactsGreet.Web.ViewModels.Articles;
    using FactsGreet.Web.ViewModels.Shared;
    using Microsoft.AspNetCore.Mvc;

    public class ArticlesController : BaseController
    {
        private const int ArticlesPerPage = 2;

        private readonly ArticlesService articlesService;

        public ArticlesController(ArticlesService articlesService)
        {
            this.articlesService = articlesService;
        }

        public async Task<IActionResult> GetByTitle(string slug)
        {
            if (slug is null)
            {
                return this.RedirectToAction(nameof(this.All));
            }

            slug = slug.Replace('_', ' ');

            var article = await this.articlesService.GetByTitleAsync<ArticleViewModel>(slug);
            if (article is null)
            {
                return this.NotFound();
            }

            return this.View("Article", article);
        }

        public async Task<IActionResult> Search(string slug, int page)
        {
            if (slug is null)
            {
                return this.RedirectToAction(nameof(this.All));
            }

            var pagination = Paginator.GetPagination(page, ArticlesPerPage);
            var articles = await this.articlesService
                .GetPaginatedByTitleKeywordsAsync<CompactArticleViewModel>(
                    pagination.Skip,
                    pagination.Take,
                    slug);

            return this.View(new SearchArticlesViewModel
            {
                Articles = articles,
                PaginationViewModel = new PaginationViewModel
                {
                    ArticlesCount = ArticlesPerPage,
                    PagesCount =
                        (int) Math.Ceiling(1.0 * await this.articlesService
                                               .GetCountByTitleKeywordsAsync(slug) /
                                           ArticlesPerPage),
                    CurrentPage = page,
                    ControllerName = nameof(ArticlesController).Replace("Controller", string.Empty),
                    ActionName = nameof(this.Search),
                    Slug = slug,
                },
                Slug = slug,
            });
        }

        public async Task<IActionResult> All(int page = 1)
        {
            var pagination = Paginator.GetPagination(page, ArticlesPerPage);
            var articles = await this.articlesService
                .GetPaginatedOrderedByDateDescendingAsync<CompactArticleViewModel>(
                    pagination.Skip,
                    pagination.Take);
            return this.View(new AllArticlesViewModel
            {
                Articles = articles,
                PaginationViewModel = new PaginationViewModel
                {
                    CurrentPage = page,
                    ArticlesCount = ArticlesPerPage,
                    PagesCount = (int) Math.Ceiling(1.0 * this.articlesService.GetCount() / ArticlesPerPage),
                    ControllerName = nameof(ArticlesController).Replace("Controller", string.Empty),
                    ActionName = nameof(this.All),
                },
            });
        }
    }
}
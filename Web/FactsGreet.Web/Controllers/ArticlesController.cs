using System.Linq;

namespace FactsGreet.Web.Controllers
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using FactsGreet.Common;
    using FactsGreet.Services.Data;
    using FactsGreet.Web.ViewModels.Articles;
    using FactsGreet.Web.ViewModels.Shared;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class ArticlesController : BaseController
    {
        private const int ArticlesPerPage = 2;

        private readonly ArticlesService articlesService;

        public ArticlesController(
            ArticlesService articlesService)
        {
            this.articlesService = articlesService;
        }

        [HttpGet("[controller]/{slug:required}")]
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
                return this.View("ArticleNotFound", slug);
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

        [HttpGet("[controller]")]
        [HttpGet("[controller]/[action]")]
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
                    PagesCount = (int) Math.Ceiling(1.0 * await this.articlesService.GetCountAsync() / ArticlesPerPage),
                    ControllerName = nameof(ArticlesController).Replace("Controller", string.Empty),
                    ActionName = nameof(this.All),
                },
            });
        }

        [Authorize]
        public async Task<IActionResult> ToggleStar(Guid id)
        {
            await this.articlesService
                .ToggleStarAsync(
                    this.User
                        .FindFirstValue(ClaimTypes.NameIdentifier), id);
            return this.Redirect("/Articles/" + (await this.articlesService.GetTitleByIdAsync(id))
                .Replace(' ', '_'));
        }

        [Authorize]
        public IActionResult Create(string title)
        {
            return this.View(new ArticleCreateInputModel {Title = title});
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(ArticleCreateInputModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            if (await this.articlesService.DoesTitleExistsAsync(model.Title))
            {
                this.ViewBag.ErrorMessage = "Title already exists";
                return this.View(model);
            }

            await this.articlesService.CreateAsync(
                this.User.FindFirstValue(ClaimTypes.NameIdentifier),
                model.Title,
                model.Content,
                model.Categories,
                model.ThumbnailLink,
                model.Description);

            return this.Redirect($"/Articles/{model.Title}");
        }
    }
}
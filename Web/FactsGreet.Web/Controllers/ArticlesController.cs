namespace FactsGreet.Web.Controllers
{
    using System;
    using System.Linq;
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

        [Route("/Article/{title}", Name = "article")]
        public async Task<IActionResult> GetByTitle(string title)
        {
            if (title is null)
            {
                return this.RedirectToAction(nameof(this.All));
            }

            var article = await this.articlesService.GetByTitleAsync<ArticleViewModel>(title);
            if (article is null)
            {
                return this.View("ArticleNotFound", title);
            }

            article.IsStarredByUser =
                await this.articlesService.IsStarredByUserAsync(
                    article.Id,
                    this.User.FindFirstValue(ClaimTypes.NameIdentifier));

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
                    ItemsCount = ArticlesPerPage,
                    PagesCount =
                        (int)Math.Ceiling(1.0 * await this.articlesService
                                              .GetCountByTitleKeywordsAsync(slug) /
                                          ArticlesPerPage),
                    CurrentPage = page,
                    Path = $"/{nameof(ArticlesController).Replace("Controller", string.Empty)}/{nameof(this.Search)}",
                    Query = { ("slug", slug) },
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
                    ItemsCount = ArticlesPerPage,
                    PagesCount = (int)Math.Ceiling(1.0 * await this.articlesService.GetCountAsync() / ArticlesPerPage),
                    Path = $"/{nameof(ArticlesController).Replace("Controller", string.Empty)}/{nameof(this.All)}",
                },
            });
        }

        [Authorize]
        public IActionResult Create(string title)
        {
            return this.View(new ArticleCreateInputModel { Title = title });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(ArticleCreateInputModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            if (await this.articlesService.DoesTitleExistAsync(model.Title))
            {
                this.ViewBag.ErrorMessage = "Title already exists";
                return this.View(model);
            }

            // TODO: upload and get link
            var thumbnailLink = string.Empty;

            await this.articlesService.CreateAsync(
                this.User.FindFirstValue(ClaimTypes.NameIdentifier),
                model.Title,
                model.Content,
                model.Categories.Select(x => x.Name).ToArray(),
                thumbnailLink,
                model.Description);

            return this.RedirectToRoute("article", new
            {
                title = model.Title,
            });
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            await this.articlesService.DeleteAsync(id);
            return this.RedirectToAction(nameof(this.All));
        }

        [Authorize]
        public async Task<IActionResult> CreateDeletionRequest(Guid id)
        {
            if (this.User.FindFirstValue(ClaimTypes.NameIdentifier) !=
                await this.articlesService.GetAuthorIdAsync(id))
            {
                return this.Unauthorized();
            }

            return this.View(new ArticleDeletionRequestCreateInputModel
            {
                Id = id,
                Article = await this.articlesService.GetByIdAsync<CompactArticleViewModel>(id),
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateDeletionRequest(ArticleDeletionRequestCreateInputModel model)
        {
            var authorId = await this.articlesService.GetAuthorIdAsync(model.Id);
            if (this.User.FindFirstValue(ClaimTypes.NameIdentifier) != authorId)
            {
                return this.Unauthorized();
            }

            await this.articlesService.CreateDeletionRequestAsync(model.Id, authorId, model.Reason);
            return this.RedirectToAction(nameof(this.All));
        }

        [Authorize]
        public async Task<IActionResult> RemoveFromStarred(Guid id)
        {
            await this.articlesService.RemoveStarAsync(this.User.FindFirstValue(ClaimTypes.NameIdentifier), id);
            return this.RedirectToRoute(
                "article",
                new
                {
                    title = await this.articlesService.GetTitleAsync(id),
                });
        }

        [Authorize]
        public async Task<IActionResult> AddToStarred(Guid id)
        {
            await this.articlesService.AddStarAsync(this.User.FindFirstValue(ClaimTypes.NameIdentifier), id);
            return this.RedirectToRoute(
                "article",
                new
                {
                    title = await this.articlesService.GetTitleAsync(id),
                });
        }
    }
}

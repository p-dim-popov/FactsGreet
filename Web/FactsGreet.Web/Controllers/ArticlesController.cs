namespace FactsGreet.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using FactsGreet.Common;
    using FactsGreet.Services.Data;
    using FactsGreet.Web.Infrastructure;
    using FactsGreet.Web.ViewModels.Articles;
    using FactsGreet.Web.ViewModels.Shared;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class ArticlesController : BaseController
    {
        private const int ArticlesPerPage = 2;

        private readonly IArticlesService articlesService;
        private readonly IFilesService filesService;
        private readonly IStarsService starsService;

        public ArticlesController(
            IArticlesService articlesService,
            IFilesService filesService,
            IStarsService starsService)
        {
            this.articlesService = articlesService;
            this.filesService = filesService;
            this.starsService = starsService;
        }

        [Route("Article/{title}", Name = nameof(ArticlesController) + nameof(GetByTitle))]
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

            // p.s.: think about this... wasting resources
            if (article.Title != title)
            {
                return this.RedirectToRoute(new { action = "GetByTitle", title = article.Title });
            }

            article.IsStarredByUser =
                await this.starsService.IsArticleStarredByUserAsync(
                    article.Id,
                    this.UserId);

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
                    pagination.Skip, pagination.Take, slug);

            var model = new SearchArticlesViewModel
            {
                Articles = articles,
                PaginationViewModel = new PaginationViewModel(
                    page,
                    ArticlesPerPage,
                    await this.articlesService.GetCountByTitleKeywordsAsync(slug),
                    allRouteDataObject: new { slug }),
                Slug = slug,
            };
            return this.View(model);
        }

        [HttpGet("[controller]")]
        [HttpGet("[controller]/[action]", Name = nameof(ArticlesController) + nameof(All))]
        public async Task<IActionResult> All(int page)
        {
            var pagination = Paginator.GetPagination(page, ArticlesPerPage);
            var articles = await this.articlesService
                .GetPaginatedOrderedByDescAsync<CompactArticleViewModel>(
                    pagination.Skip, pagination.Take);

            return this.View(new AllArticlesViewModel
            {
                Articles = articles,
                PaginationViewModel = new PaginationViewModel(
                    page, ArticlesPerPage, await this.articlesService.GetCountAsync()),
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

            string thumbnailLink;
            try
            {
                thumbnailLink = model.ThumbnailLink ??
                                (model.ThumbnailImage is null
                                    ? null
                                    : await this.filesService.UploadAsync(
                                        model.ThumbnailImage.OpenReadStream(),
                                        model.ThumbnailImage.FileName,
                                        this.UserId));
            }
            catch (InvalidOperationException)
            {
                this.ViewBag.ErrorMessage = "No more available storage. Please delete some files to upload new ones";
                return this.View(model);
            }

            await this.articlesService.CreateAsync(
                this.UserId,
                model.Title,
                model.Content,
                model.Categories.Select(x => x.Name).ToArray(),
                thumbnailLink,
                model.Description);

            return this.RedirectToRoute(new
            {
                action = "GetByTitle",
                title = model.Title,
            });
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            await this.articlesService.DeleteAsync(id);
            return this.RedirectToRoute(
                new
                {
                    area = "Administration",
                    controller = "Dashboard",
                    action = "ArticleDeletionRequests",
                });
        }

        [Authorize]
        public async Task<IActionResult> CreateDeletionRequest(Guid id)
        {
            if (this.UserId != await this.articlesService.GetAuthorIdAsync(id))
            {
                return this.Forbid();
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
            if (this.UserId != authorId)
            {
                return this.Forbid();
            }

            await this.articlesService.CreateDeletionRequestAsync(model.Id, this.UserId, model.Reason);
            return this.RedirectToAction(nameof(this.All));
        }

        [Authorize]
        public async Task<IActionResult> RemoveFromStarred(Guid id)
        {
            await this.starsService.RemoveStarLinkAsync(this.UserId, id);
            return this.RedirectToRoute(
                new
                {
                    action = "GetByTitle",
                    title = await this.articlesService.GetTitleAsync(id),
                });
        }

        [Authorize]
        public async Task<IActionResult> AddToStarred(Guid id)
        {
            await this.starsService.CreateStarLinkAsync(this.UserId, id);
            return this.RedirectToRoute(
                new
                {
                    action = "GetByTitle",
                    title = await this.articlesService.GetTitleAsync(id),
                });
        }
    }
}

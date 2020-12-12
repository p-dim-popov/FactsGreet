namespace FactsGreet.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Data;
    using FactsGreet.Services.Data.Implementations;
    using FactsGreet.Services.Mapping;
    using FactsGreet.Web.Infrastructure;
    using FactsGreet.Web.Infrastructure.Attributes;
    using FactsGreet.Web.ViewModels.Articles;
    using FactsGreet.Web.ViewModels.Edits;
    using FactsGreet.Web.ViewModels.Shared;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class EditsController : BaseController
    {
        private const int EditsPerPage = 3;

        private readonly IEditsService editsService;
        private readonly IArticlesService articlesService;
        private readonly IFilesService filesService;

        public EditsController(
            IEditsService editsService,
            IArticlesService articlesService,
            IFilesService filesService)
        {
            this.editsService = editsService;
            this.articlesService = articlesService;
            this.filesService = filesService;
        }

        [Authorize]
        [Route("[controller]/[action]/{title}", Name = nameof(EditsController) + nameof(Create) + "GET")]
        public async Task<IActionResult> Create(string title)
        {
            var article = await this.articlesService.GetByTitleAsync<ArticleCreateEditInputModel>(title);
            if (article is null)
            {
                return this.NotFound();
            }

            return this.View(new EditCreateInputModel
            {
                Article = article,
            });
        }

        [Authorize]
        [HttpPost("[controller]/[action]/{title}")]
        public async Task<IActionResult> Create(EditCreateInputModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            /* TODO: concurency check */

            if (model.Article.Title != await this.articlesService.GetTitleAsync(model.Article.Id))
            {
                if (await this.articlesService.DoesTitleExistAsync(model.Article.Title))
                {
                    this.ViewBag.ErrorMessage = "Title already exists";
                    return this.View(model);
                }
            }

            string thumbnailLink;
            try
            {
                thumbnailLink = model.Article.ThumbnailLink ??
                                (model.Article.ThumbnailImage is null
                                    ? null
                                    : await this.filesService.UploadAsync(
                                        model.Article.ThumbnailImage.OpenReadStream(),
                                        model.Article.ThumbnailImage.FileName,
                                        this.UserId));
            }
            catch (InvalidOperationException)
            {
                this.ViewBag.ErrorMessage = "No more available storage. Please delete some files to upload new ones";
                return this.View(model);
            }

            await this.editsService.CreateAsync(
                model.Article.Id,
                this.UserId,
                model.Article.Title,
                model.Article.Content,
                model.Article.Description,
                model.Article.Categories.Select(x => x.Name).ToArray(),
                thumbnailLink,
                model.Comment);

            return this.RedirectToRoute(new
            {
                controller = "Articles",
                action = "GetByTitle",
                title = model.Article.Title,
            });
        }

        [Route("[controller]/[action]/{title}", Name = nameof(EditsController) + nameof(History))]
        public async Task<IActionResult> History(string title, int page)
        {
            var pagination = Paginator.GetPagination(page, EditsPerPage);
            return this.View(new HistoryViewModel
            {
                ArticleTitle = title,
                Edits = await this.editsService.GetPaginatedOrderByDescAsync<CompactEditViewModel>(
                    pagination.Skip, pagination.Take, filter: x => x.Article.Title == title),
                PaginationViewModel = new CompactPaginationViewModel(
                    page,
                    typeof(EditsController),
                    nameof(this.History),
                    new { title }),
            });
        }

        public async Task<IActionResult> GetEditsInfoList(
            Guid id,
            char which,
            int page)
        {
            var pagination = Paginator.GetPagination(page, EditsPerPage);
            var creationDate = await this.editsService.GetCreationDateAsync(id);
            var articleId = await this.editsService.GetArticleIdAsync(id);
            var edits = which switch
            {
                '>' => await this.editsService
                    .GetEditsInfoListNewerThan<CompactEditViewModel>(
                        pagination.Skip, pagination.Take, articleId, creationDate),
                '<' => await this.editsService
                    .GetEditsInfoListOlderThan<CompactEditViewModel>(
                        pagination.Skip, pagination.Take, articleId, creationDate),
                _ => null,
            };

            return this.Json(edits);
        }

        public async Task<IActionResult> View(Guid id, Guid? against)
        {
            return this.View(EditViewModel.CreateFrom(await this.editsService.GetByIdAsync(id, against)));
        }

        [Authorize]
        public async Task<IActionResult> GetEditsWithArticleCards(int page, string userId = null)
        {
            var pagination = Paginator.GetPagination(page, EditsPerPage);
            var edits =
                await this.editsService.GetPaginatedOrderByDescAsync<EditWithCompactArticleViewModel>(
                    pagination.Skip,
                    pagination.Take,
                    userId);

            return this.PartialView("_ListCompactEditsPartial", edits);
        }
    }
}

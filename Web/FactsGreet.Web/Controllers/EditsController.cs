namespace FactsGreet.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Data;
    using FactsGreet.Web.Infrastructure;
    using FactsGreet.Web.ViewModels.Articles;
    using FactsGreet.Web.ViewModels.Edits;
    using FactsGreet.Web.ViewModels.Shared;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class EditsController : BaseController
    {
        private const int EditsPerPage = 3;

        private readonly EditsService editsService;
        private readonly ArticlesService articlesService;
        private readonly FilesService filesService;

        public EditsController(
            EditsService editsService,
            ArticlesService articlesService,
            FilesService filesService)
        {
            this.editsService = editsService;
            this.articlesService = articlesService;
            this.filesService = filesService;
        }

        [Authorize]
        [HttpGet("[controller]/[action]/{title}", Name = "edits_create")]
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
                                        model.Article.ThumbnailImage.Length,
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

            return this.RedirectToRoute("article", new { title = model.Article.Title });
        }

        [Route("[controller]/[action]/{title}", Name = "edits_history")]
        public async Task<IActionResult> History(string title, int page = 1)
        {
            return this.View(new HistoryViewModel
            {
                ArticleTitle = title,
                Edits = await this.GetEditsPagedOrderByDescAsync<CompactEditViewModel>(
                    page, filter: x => x.Article.Title == title),
                PaginationViewModel = new CompactPaginationViewModel { CurrentPage = page },
            });
        }

        public async Task<IActionResult> GetEditsInfoList(
            Guid id,
            char which,
            int page = 1)
        {
            var creationDate = await this.editsService.GetCreationDateAsync(id);
            var articleId = await this.editsService.GetArticleIdAsync(id);
            var edits = which switch
            {
                '>' => await this.GetEditsInfoListNewerThan<CompactEditViewModel>(articleId, creationDate, page),
                '<' => await this.GetEditsInfoListOlderThan<CompactEditViewModel>(articleId, creationDate, page),
                _ => null,
            };

            return this.Json(edits);
        }

        public async Task<IActionResult> View(Guid id, Guid? against)
        {
            return this.View(EditViewModel.CreateFrom(await this.editsService.GetByIdAsync(id, against)));
        }

        [Authorize]
        public async Task<IActionResult> GetEditsWithArticleCards(int page = 1, string userId = null)
        {
            var edits =
                await this.GetEditsPagedOrderByDescAsync<EditWithCompactArticleViewModel>(page, userId);

            return this.PartialView("_ListCompactEditsPartial", edits);
        }

        private Task<ICollection<T>> GetEditsPagedOrderByDescAsync<T>(
            int page = 1,
            string userId = null,
            Expression<Func<Edit, bool>> filter = null)
        {
            return this.GetEditsPagedOrderByDescAsync<T, DateTime>(page, userId, filter);
        }

        private async Task<ICollection<T>> GetEditsPagedOrderByDescAsync<T, TOrderKey>(
            int page = 1,
            string userId = null,
            Expression<Func<Edit, bool>> filter = null,
            Expression<Func<Edit, TOrderKey>> order = null)
        {
            var pagination = Paginator.GetPagination(page, EditsPerPage);

            return await this.editsService.GetPaginatedOrderByDescAsync<T, TOrderKey>(
                pagination.Skip,
                pagination.Take,
                userId,
                filter,
                order);
        }

        private async Task<ICollection<T>> GetEditsInfoListNewerThan<T>(
            Guid articleId,
            DateTime creationDate,
            int page)
        {
            var edits = await this.GetEditsPagedOrderByDescAsync<T>(
                page,
                filter: x =>
                    x.Article.Id == articleId && x.CreatedOn > creationDate);
            return edits;
        }

        private async Task<ICollection<T>> GetEditsInfoListOlderThan<T>(
            Guid articleId,
            DateTime creationDate,
            int page)
        {
            var edits = await this.GetEditsPagedOrderByDescAsync<T>(
                page,
                filter: x =>
                    x.Article.Id == articleId && x.CreatedOn < creationDate);
            return edits;
        }
    }
}

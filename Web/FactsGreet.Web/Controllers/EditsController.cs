namespace FactsGreet.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using DiffMatchPatch;
    using FactsGreet.Common;
    using FactsGreet.Data.Models;
    using FactsGreet.Data.Models.Enums;
    using FactsGreet.Services.Data;
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

        public EditsController(EditsService editsService, ArticlesService articlesService)
        {
            this.editsService = editsService;
            this.articlesService = articlesService;
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

            var dmp = new diff_match_patch();
            var index = 0;
            var diffs = dmp.diff_main(
                    await this.articlesService.GetContentAsync(model.Article.Id),
                    model.Article.Content)
                .Select(x =>
                (
                    Index: index++,
                    Operation: (DiffOperation)x.operation,
                    Text: x.text.ToString()))
                .ToArray();

            // TODO: upload and get link
            var thumbnailLink = string.Empty;

            await this.editsService.CreateAsync(
                model.Article.Id,
                this.User.FindFirstValue(ClaimTypes.NameIdentifier),
                model.Article.Title,
                model.Article.Content,
                model.Article.Description,
                model.Article.Categories.Select(x => x.Name).ToArray(),
                thumbnailLink,
                model.Comment,
                diffs);

            return this.RedirectToRoute("article", new { title = model.Article.Title });
        }

        [Route("[controller]/[action]/{title}", Name = "edits_history")]
        public async Task<IActionResult> History(string title, int page = 1)
        {
            return this.View(new HistoryViewModel
            {
                Article = await this.articlesService.GetByTitleAsync<ArticleWithEditsViewModel>(title),
                PaginationViewModel = new CompactPaginationViewModel { CurrentPage = page },
            });
        }

        public async Task<IActionResult> View(Guid id)
        {
            return this.View(await this.editsService.GetById<EditViewModel>(id));
        }

        [Authorize]
        public async Task<IActionResult> GetEdits(int page = 1, string userId = null)
        {
            var pagination = Paginator.GetPagination(page, EditsPerPage);

            var edits =
                await this.editsService
                    .GetPaginatedOrderedByDateDescendingAsync<CompactEditViewModel>(
                        pagination.Skip, pagination.Take, userId);

            return this.PartialView("_ListCompactEditsPartial", edits);
        }
    }
}

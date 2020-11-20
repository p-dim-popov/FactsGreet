﻿namespace FactsGreet.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text.Json;
    using System.Threading.Tasks;
    using AutoMapper;
    using FactsGreet.Data.Models;
    using FactsGreet.Services;
    using FactsGreet.Services.Data;
    using FactsGreet.Web.Infrastructure;
    using FactsGreet.Web.ViewModels.Articles;
    using FactsGreet.Web.ViewModels.Edits;
    using FactsGreet.Web.ViewModels.Shared;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    public class EditsController : BaseController
    {
        private const int EditsPerPage = 3;

        private readonly EditsService editsService;
        private readonly ArticlesService articlesService;
        private readonly FilesService filesService;
        private readonly DiffMatchPatchService diffMatchPatchService;

        public EditsController(
            EditsService editsService,
            ArticlesService articlesService,
            FilesService filesService,
            DiffMatchPatchService diffMatchPatchService)
        {
            this.editsService = editsService;
            this.articlesService = articlesService;
            this.filesService = filesService;
            this.diffMatchPatchService = diffMatchPatchService;
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
                Edits = await this.GetEditsPagedOrderByDescAsync<CompactEditViewModel, DateTime>(
                    page, filter: x => x.Article.Title == title),
                PaginationViewModel = new CompactPaginationViewModel { CurrentPage = page },
            });
        }

        public async Task<IActionResult> GetEditsInfoList(
            string title,
            char which,
            DateTime target,
            int page = 1)
        {
            // TODO: add select to corresponding view
            return this.Json(
                await this.GetEditsPagedOrderByDescAsync<CompactEditViewModel, DateTime>(
                    page,
                    filter: x =>
                        x.Article.Title == title &&
                        which == '>'
                            ? x.CreatedOn > target

                            // ReSharper disable once SimplifyConditionalTernaryExpression
                            : which == '<'
                                ? x.CreatedOn < target
                                : false));
        }

        public async Task<IActionResult> View(Guid id, Guid? against)
        {
            return this.View(new EditViewModel(await this.editsService.GetByIdAsync(id, against)));
        }

        [Authorize]
        public async Task<IActionResult> GetEditsWithArticleCards(int page = 1, string userId = null)
        {
            var edits =
                await this.GetEditsPagedOrderByDescAsync<EditWithCompactArticleViewModel, DateTime>(page, userId);

            return this.PartialView("_ListCompactEditsPartial", edits);
        }

        private async Task<ICollection<T>> GetEditsPagedOrderByDescAsync<T, TOrderKey>(
            int page = 1,
            string userId = null,
            Expression<Func<Edit, bool>> filter = null,
            Expression<Func<Edit, TOrderKey>> order = null)
        {
            var pagination = Paginator.GetPagination(page, EditsPerPage);

            return await this.editsService.GetPaginatedOrderedByDescAsync<T, TOrderKey>(
                pagination.Skip,
                pagination.Take,
                userId,
                filter,
                order);
        }
    }
}

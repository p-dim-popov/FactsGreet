namespace FactsGreet.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FactsGreet.Data.Common.Repositories;
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Data.TransferObjects;
    using FactsGreet.Services.Mapping;
    using Microsoft.EntityFrameworkCore;
    using TrueCommerce.Shared.DiffMatchPatch;
    using DbDiff = FactsGreet.Data.Models.Diff;
    using DbPatch = FactsGreet.Data.Models.Patch;
    using DmpDiff = TrueCommerce.Shared.DiffMatchPatch.Diff;
    using DmpPatch = TrueCommerce.Shared.DiffMatchPatch.Patch;

    public class EditsService
    {
        private readonly IDeletableEntityRepository<Edit> editRepository;
        private readonly IDeletableEntityRepository<Category> categoryRepository;
        private readonly IDeletableEntityRepository<Article> articleRepository;
        private readonly DiffMatchPatchService diffMatchPatchService;

        public EditsService(
            IDeletableEntityRepository<Edit> editRepository,
            IDeletableEntityRepository<Category> categoryRepository,
            IDeletableEntityRepository<Article> articleRepository,
            DiffMatchPatchService diffMatchPatchService)
        {
            this.editRepository = editRepository;
            this.categoryRepository = categoryRepository;
            this.articleRepository = articleRepository;
            this.diffMatchPatchService = diffMatchPatchService;
        }

        public async Task<ICollection<T>> GetPaginatedOrderedByDateDescendingAsync<T>(
            int skip,
            int take,
            string userId = null)
        {
            var edits = this.editRepository.All();

            if (userId is { })
            {
                edits = edits.Where(x => x.EditorId == userId);
            }

            return await edits
                .OrderByDescending(x => x.CreatedOn)
                .To<T>()
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<EditDto> GetById(Guid id, Guid? against = null)
        {
            var againstDate = against.HasValue
                ? await this.editRepository
                    .AllAsNoTracking()
                    .Where(x => x.Id == id)
                    .Select(x => x.CreatedOn)
                    .FirstOrDefaultAsync()
                : DateTime.UtcNow;

            var targetDate = await this.editRepository
                .AllAsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => x.CreatedOn)
                .FirstOrDefaultAsync();

            var laterEdits = await this.editRepository
                .AllAsNoTracking()
                .OrderBy(x => x.CreatedOn)
                .Where(x => x.CreatedOn >= targetDate)
                .Include(x => x.Patches)
                .ThenInclude(x => x.Diffs)
                .AsSingleQuery()
                .Select(x => new
                {
                    x.CreatedOn,
                    x.Patches,
                })
                .ToListAsync();

            var patches = laterEdits.Aggregate(
                new { LaterEdits = new List<List<DbPatch>>(), AgainstToCurrentEdits = new List<List<DbPatch>>() },
                (acc, cur) =>
                {
                    if (cur.CreatedOn <= againstDate)
                    {
                        acc.LaterEdits.Add(cur.Patches.ToList());
                    }
                    else
                    {
                        acc.AgainstToCurrentEdits.Add(cur.Patches.ToList());
                    }

                    return acc;
                });

            var presentArticle = await this.editRepository
                .AllAsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => x.Article.Content)
                .FirstOrDefaultAsync();

            var againstArticle =
                this.diffMatchPatchService
                    .ApplyEdits(presentArticle, patches.AgainstToCurrentEdits);

            var targetArticle =
                this.diffMatchPatchService
                    .ApplyEdits(againstArticle, patches.LaterEdits);

            return new EditDto
            {
                ArticleContent = targetArticle,
            };
        }

        public async Task CreateAsync(
            Guid articleId,
            string editorId,
            string newTitle,
            string newContent,
            string newDescription,
            string[] newCategories,
            string newThumbnailLink,
            string editComment)
        {
            // TODO: pls have time to optimize this...
            newCategories = newCategories
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.ToLowerInvariant())
                .ToArray();

            var newCategoriesFromDb = await this.categoryRepository
                .All()
                .Where(x => newCategories.Contains(x.Name))
                .ToListAsync();

            var newCategoriesNotFromDb = newCategories
                .Except(newCategoriesFromDb.Select(x => x.Name))
                .Select(x => new Category { Name = x })
                .ToList();

            var article = await this.articleRepository.All()
                .Include(x => x.Categories)
                .Where(x => x.Id == articleId)
                .FirstOrDefaultAsync();

            article.Categories = article.Categories

                // filter out the removed categories
                .Where(x => newCategories.Contains(x.Name))

                // add existing in db categories
                .Concat(newCategoriesFromDb)

                // add new db categories
                .Concat(newCategoriesNotFromDb)
                .ToList();

            article.Title = newTitle;
            article.Content = newContent;
            article.Description = newDescription;
            article.ThumbnailLink = newThumbnailLink ?? article.ThumbnailLink;
            article.Edits.Add(new Edit
            {
                Comment = editComment,
                EditorId = editorId,
                Patches = this.diffMatchPatchService.CreateEdit(
                    await this.articleRepository
                        .AllAsNoTracking()
                        .Where(x => x.Id == articleId)
                        .Select(x => x.Content)
                        .FirstOrDefaultAsync(),
                    newContent),
                Notification =
                {
                    SenderId = editorId,
                },
            });

            await this.articleRepository.SaveChangesAsync();
        }
    }
}

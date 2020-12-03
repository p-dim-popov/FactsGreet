namespace FactsGreet.Services.Data.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using FactsGreet.Data.Common.Repositories;
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Data.TransferObjects.Edits;
    using FactsGreet.Services.Mapping;
    using Microsoft.EntityFrameworkCore;
    using DbPatch = FactsGreet.Data.Models.Patch;

    public class EditsService
    {
        private readonly IDeletableEntityRepository<Edit> editRepository;
        private readonly IDeletableEntityRepository<Category> categoryRepository;
        private readonly IDeletableEntityRepository<Article> articleRepository;
        private readonly IDiffMatchPatchService diffMatchPatchService;

        public EditsService(
            IDeletableEntityRepository<Edit> editRepository,
            IDeletableEntityRepository<Category> categoryRepository,
            IDeletableEntityRepository<Article> articleRepository,
            IDiffMatchPatchService diffMatchPatchService)
        {
            this.editRepository = editRepository;
            this.categoryRepository = categoryRepository;
            this.articleRepository = articleRepository;
            this.diffMatchPatchService = diffMatchPatchService;
        }

        public async Task<ICollection<T>> GetPaginatedOrderByDescAsync<T, TOrderKey>(
            int skip,
            int take,
            string userId = null,
            Expression<Func<Edit, bool>> filter = null,
            Expression<Func<Edit, TOrderKey>> order = null)
        {
            var edits = this.editRepository.All();

            if (userId is not null)
            {
                edits = edits.Where(x => x.EditorId == userId);
            }

            if (filter is not null)
            {
                edits = edits.Where(filter);
            }

            edits = order is not null
                ? edits.OrderByDescending(order)
                : edits.OrderByDescending(x => x.CreatedOn);

            return await edits
                .To<T>()
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<EditDto> GetByIdAsync(Guid targetId, Guid? againstId = null)
        {
            againstId ??= await this.editRepository.AllAsNoTracking()
                .OrderByDescending(x => x.CreatedOn)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            /* For some reason To<> won't work... Too bad */

            var againstEdit = await this.editRepository
                .AllAsNoTracking()
                .Where(x => x.Id == againstId)
                .Select(x => new CompactEditDto
                {
                    Id = x.Id,
                    Comment = x.Comment,
                    CreatedOn = x.CreatedOn,
                    EditorUserName = x.Editor.UserName,
                })
                .FirstOrDefaultAsync();

            var targetEdit = await this.editRepository
                .AllAsNoTracking()
                .Where(x => x.Id == targetId)
                .Select(x => new CompactEditDto
                {
                    Id = x.Id,
                    Comment = x.Comment,
                    CreatedOn = x.CreatedOn,
                    EditorUserName = x.Editor.UserName,
                })
                .FirstOrDefaultAsync();

            // all edits made after the requested edit
            var laterEdits = (await this.editRepository
                    .AllAsNoTracking()
                    .OrderBy(x => x.CreatedOn)
                    .Where(x => x.CreatedOn > targetEdit.CreatedOn)
                    .Include(x => x.Patches)
                    .ThenInclude(x => x.Diffs)
                    .AsSingleQuery()
                    .Select(x => new
                    {
                        x.CreatedOn,
                        x.Patches,
                    })
                    .ToListAsync())
                .Aggregate(
                    new
                    {
                        AgainstToTarget = new List<List<DbPatch>>(),
                        LatestToAgainst = new List<List<DbPatch>>(),
                    },
                    (acc, cur) =>
                    {
                        if (cur.CreatedOn > againstEdit.CreatedOn)
                        {
                            acc.LatestToAgainst.Add(cur.Patches.ToList());
                        }
                        else
                        {
                            acc.AgainstToTarget.Add(cur.Patches.ToList());
                        }

                        return acc;
                    });

            var presentArticle = await this.editRepository
                .AllAsNoTracking()
                .Where(x => x.Id == targetId)
                .Select(x => new { x.Article.Content, x.Article.Title })
                .FirstOrDefaultAsync();

            var againstArticle =
                this.diffMatchPatchService
                    .ApplyEdits(presentArticle.Content, laterEdits.LatestToAgainst);

            var targetArticle =
                this.diffMatchPatchService
                    .ApplyEdits(againstArticle, laterEdits.AgainstToTarget);

            return new EditDto
            {
                TargetArticleContent = targetArticle,
                AgainstArticleContent = againstArticle,
                ArticleTitle = presentArticle.Title,
                TargetEdit = targetEdit,
                AgainstEdit = againstEdit,
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
            });

            await this.articleRepository.SaveChangesAsync();
        }

        public Task<DateTime> GetCreationDateAsync(Guid id)
        {
            return this.editRepository.AllAsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => x.CreatedOn)
                .FirstOrDefaultAsync();
        }

        public Task<Guid> GetArticleIdAsync(Guid id)
        {
            return this.editRepository.AllAsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => x.Article.Id)
                .FirstOrDefaultAsync();
        }
    }
}

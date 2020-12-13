namespace FactsGreet.Services.Data.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using FactsGreet.Data.Common.Repositories;
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Data.TransferObjects.ApplicationUsers;
    using FactsGreet.Services.Data.TransferObjects.Articles;
    using FactsGreet.Services.Data.TransferObjects.Edits;
    using FactsGreet.Services.Mapping;
    using Microsoft.EntityFrameworkCore;
    using DbPatch = FactsGreet.Data.Models.Patch;

    public class EditsService : IEditsService
    {
        private readonly IDeletableEntityRepository<Edit> editRepository;
        private readonly IDeletableEntityRepository<Category> categoryRepository;
        private readonly IDeletableEntityRepository<Article> articleRepository;
        private readonly IDiffMatchPatchService diffMatchPatchService;
        private readonly IFollowsService followsService;
        private readonly IStarsService starsService;

        public EditsService(
            IDeletableEntityRepository<Edit> editRepository,
            IDeletableEntityRepository<Category> categoryRepository,
            IDeletableEntityRepository<Article> articleRepository,
            IDiffMatchPatchService diffMatchPatchService,
            IFollowsService followsService,
            IStarsService starsService)
        {
            this.editRepository = editRepository;
            this.categoryRepository = categoryRepository;
            this.articleRepository = articleRepository;
            this.diffMatchPatchService = diffMatchPatchService;
            this.followsService = followsService;
            this.starsService = starsService;
        }

        public Task<ICollection<T>> GetEditsNewerThan<T>(
            int skip,
            int take,
            Guid articleId,
            DateTime creationDate)
            where T : IMapFrom<Edit>
        {
            var edits = this.GetPaginatedOrderByDescAsync<T>(
                skip,
                take,
                filter: x =>
                    x.Article.Id == articleId && x.CreatedOn > creationDate);
            return edits;
        }

        public Task<ICollection<T>> GetEditsOlderThan<T>(
            int skip,
            int take,
            Guid articleId,
            DateTime creationDate)
            where T : IMapFrom<Edit>
        {
            var edits = this.GetPaginatedOrderByDescAsync<T>(
                skip,
                take,
                filter: x =>
                    x.Article.Id == articleId && x.CreatedOn < creationDate);
            return edits;
        }

        public async Task<ICollection<T>> GetFewOlderThanAsync<T>(
            Guid? referenceId, int take, string userId, bool forCurrentUser)
        where T : IMapFrom<Edit>
        {
            var query = this.editRepository.AllAsNoTracking();

            if (referenceId.HasValue)
            {
                var referenceDate = await this.editRepository.AllAsNoTracking()
                    .Where(x => x.Id == referenceId.Value)
                    .Select(x => x.CreatedOn)
                    .FirstOrDefaultAsync();

                query = query.Where(x => x.CreatedOn < referenceDate);
            }

            if (forCurrentUser)
            {
                var follows = (await this.followsService
                        .GetFollowedUsers<ApplicationUserWithId>(userId))
                    .Select(x => x.Id)
                    .ToList();

                var starredArticles = (await this.starsService
                        .GetAllStarredByUser<ArticleWithId>(userId))
                    .Select(x => x.Id)
                    .ToList();

                query = query.Where(x => follows.Contains(x.EditorId) || starredArticles.Contains(x.ArticleId));
            }
            else
            {
                query = query.Where(x => x.EditorId == userId);
            }

            return await query
                .OrderByDescending(x => x.CreatedOn)
                .Take(take)
                .To<T>()
                .ToListAsync();
        }

        public async Task<ICollection<T>> GetPaginatedOrderByDescAsync<T>(
            int skip,
            int take,
            string userId = null,
            Expression<Func<Edit, bool>> filter = null)
            where T : IMapFrom<Edit>
        {
            var edits = this.editRepository.AllAsNoTracking();

            if (filter is not null)
            {
                edits = edits.Where(filter);
            }

            return await edits
                .OrderByDescending(x => x.CreatedOn)
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
                    .OrderByDescending(x => x.CreatedOn)
                    .Where(x => x.CreatedOn > targetEdit.CreatedOn)
                    .Include(x => x.Patches.OrderBy(y => y.Index))
                    .ThenInclude(x => x.Diffs.OrderBy(y => y.Index))
                    .AsSingleQuery()
                    .Select(x => new
                    {
                        x.CreatedOn, Patches = x.Patches.OrderBy(y => y.Index).ToList(),
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

            var againstArticleContent =
                this.diffMatchPatchService
                    .ApplyEdits(presentArticle.Content, laterEdits.LatestToAgainst);

            var targetArticleContent =
                this.diffMatchPatchService
                    .ApplyEdits(againstArticleContent, laterEdits.AgainstToTarget);

            return new EditDto
            {
                TargetArticleContent = targetArticleContent,
                AgainstArticleContent = againstArticleContent,
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
            => this.editRepository.AllAsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => x.CreatedOn)
                .FirstOrDefaultAsync();

        public Task<Guid> GetArticleIdAsync(Guid id)
            => this.editRepository.AllAsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => x.Article.Id)
                .FirstOrDefaultAsync();
    }
}

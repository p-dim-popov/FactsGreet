namespace FactsGreet.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using FactsGreet.Data.Common.Repositories;
    using FactsGreet.Data.Models;
    using FactsGreet.Data.Models.Enums;
    using FactsGreet.Services.Mapping;
    using Microsoft.EntityFrameworkCore;

    public class ArticlesService
    {
        private readonly IDeletableEntityRepository<Article> articleRepository;
        private readonly IRepository<Category> categoryRepository;
        private readonly IRepository<Star> starRepository;
        private readonly IDeletableEntityRepository<ArticleDeletionRequest> articleDeletionRequestRepository;
        private readonly IDeletableEntityRepository<Edit> editRepository;
        private readonly DiffMatchPatchService diffMatchPatchService;

        public ArticlesService(
            IDeletableEntityRepository<Article> articleRepository,
            IRepository<Category> categoryRepository,
            IRepository<Star> starRepository,
            IDeletableEntityRepository<ArticleDeletionRequest> articleDeletionRequestRepository,
            IDeletableEntityRepository<Edit> editRepository,
            DiffMatchPatchService diffMatchPatchService)
        {
            this.articleRepository = articleRepository;
            this.categoryRepository = categoryRepository;
            this.starRepository = starRepository;
            this.articleDeletionRequestRepository = articleDeletionRequestRepository;
            this.editRepository = editRepository;
            this.diffMatchPatchService = diffMatchPatchService;
        }

        public Task<string> GetTitleAsync(Guid id)
        {
            return this.articleRepository.AllAsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => x.Title)
                .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(
            string authorId,
            string title,
            string content,
            string[] categories,
            string thumbnailLink,
            string description)
        {
            categories = categories
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.ToLowerInvariant())
                .ToArray();

            var existingCategories = await this.categoryRepository
                .All()
                .Where(x => categories.Contains(x.Name))
                .ToListAsync();

            var newCategories = categories
                .Except(existingCategories.Select(x => x.Name))
                .Select(x => new Category { Name = x })
                .ToList();

            var article = new Article
            {
                AuthorId = authorId,
                Title = title,
                Content = content,
                Description = description,
                Categories = existingCategories
                    .Concat(newCategories)
                    .ToList(),
                Edits = new List<Edit>
                {
                    new Edit
                    {
                        EditorId = authorId,
                        IsCreation = true,
                        Patches = this.diffMatchPatchService.CreateEdit(string.Empty, content),
                        Notification = { SenderId = authorId },
                        Comment = "Initial create", // TODO: Think about combining edit and create view bcs of this...
                    },
                },
            };
            if (thumbnailLink is { })
            {
                article.ThumbnailLink = thumbnailLink;
            }

            await this.articleRepository.AddAsync(article);
            await this.articleRepository.SaveChangesAsync();
        }

        public async Task AddStarAsync(string userId, Guid articleId)
        {
            var isStarred = await this.starRepository.AllAsNoTracking()
                .AnyAsync(x => x.UserId == userId && x.ArticleId == articleId);

            if (!isStarred)
            {
                await this.starRepository.AddAsync(new Star { ArticleId = articleId, UserId = userId });
                await this.articleRepository.SaveChangesAsync();
            }
        }

        public async Task RemoveStarAsync(string userId, Guid articleId)
        {
            var isStarred = await this.starRepository.AllAsNoTracking()
                .AnyAsync(x => x.UserId == userId && x.ArticleId == articleId);

            if (isStarred)
            {
                this.starRepository.Delete(new Star { ArticleId = articleId, UserId = userId });
                await this.articleRepository.SaveChangesAsync();
            }
        }

        public async Task<ICollection<T>> GetPaginatedByTitleKeywordsAsync<T>(int skip, int take, string keywords)
        {
            return await Regex.Matches(keywords, @"\""([^)]+)\""|([^\s"".?!]+)")
                .Select(x => x.Value.Replace("\"", string.Empty))
                .Aggregate(
                    this.articleRepository.AllAsNoTracking(),
                    (current, keyword)
                        => current.Where(x => x.Title.Contains(keyword)))
                .To<T>()
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetCountByTitleKeywordsAsync(string keywords)
        {
            return await Regex.Matches(keywords, @"\""([^)]+)\""|([^\s"".?!]+)")
                .Select(x => x.Value)
                .Aggregate(this.articleRepository.AllAsNoTracking(), (current, keyword)
                    => current.Where(x => x.Title.Contains(keyword)))
                .CountAsync();
        }

        public async Task<ICollection<T>> GetPaginatedOrderedByDateDescendingAsync<T>(int skip, int take)
        {
            return await this.articleRepository.AllAsNoTracking()
                .OrderByDescending(x => x.ModifiedOn)
                .To<T>()
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<T> GetByTitleAsync<T>(string title)
        {
            return await this.articleRepository.AllAsNoTracking()
                .Where(x => x.Title == title)
                .To<T>()
                .FirstOrDefaultAsync();
        }

        public Task<int> GetCountAsync()
        {
            return this.articleRepository.AllAsNoTracking().CountAsync();
        }

        public Task<bool> DoesTitleExistAsync(string title)
        {
            return this.articleRepository.AllAsNoTracking()
                .AnyAsync(x => x.Title == title);
        }

        public async Task CreateDeletionRequestAsync(Guid id, string authorId, string reason)
        {
            await this.articleDeletionRequestRepository.AddAsync(new ArticleDeletionRequest
            {
                ArticleId = id,
                Notification = { SenderId = authorId },
                Reason = reason,
            });

            await this.articleDeletionRequestRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var article = await this.articleRepository.All()
                .Include(x => x.DeletionRequests)
                .Include(x => x.Stars)
                .Include(x => x.Edits)
                .FirstOrDefaultAsync(x => x.Id == id);

            foreach (var request in article.DeletionRequests)
            {
                this.articleDeletionRequestRepository.Delete(request);
            }

            foreach (var star in article.Stars)
            {
                this.starRepository.Delete(star);
            }

            foreach (var edit in article.Edits)
            {
                this.editRepository.Delete(edit);
            }

            this.articleRepository.Delete(article);

            await this.articleRepository.SaveChangesAsync();
        }

        public Task<string> GetAuthorIdAsync(Guid id)
            => this.articleRepository.All().Where(x => x.Id == id)
                .Select(x => x.AuthorId)
                .FirstOrDefaultAsync();

        public Task<T> GetByIdAsync<T>(Guid id)
        {
            return this.articleRepository.AllAsNoTracking()
                .Where(x => x.Id == id)
                .To<T>()
                .FirstOrDefaultAsync();
        }

        public Task<bool> IsStarredByUserAsync(Guid articleId, string userId)
            => this.starRepository.AllAsNoTracking()
                .AnyAsync(x => x.ArticleId == articleId && x.UserId == userId);

        public Task<string> GetContentAsync(Guid id)
            => this.articleRepository.AllAsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => x.Content)
                .FirstOrDefaultAsync();
    }
}

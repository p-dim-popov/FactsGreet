namespace FactsGreet.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FactsGreet.Data.Common.Repositories;
    using FactsGreet.Data.Models;
    using FactsGreet.Data.Models.Enums;
    using FactsGreet.Services.Mapping;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;

    public class EditsService
    {
        private readonly IDeletableEntityRepository<Edit> editsRepository;
        private readonly IDeletableEntityRepository<Category> categoryRepository;
        private readonly IDeletableEntityRepository<Article> articleRepository;

        public EditsService(
            IDeletableEntityRepository<Edit> editsRepository,
            IDeletableEntityRepository<Category> categoryRepository,
            IDeletableEntityRepository<Article> articleRepository)
        {
            this.editsRepository = editsRepository;
            this.categoryRepository = categoryRepository;
            this.articleRepository = articleRepository;
        }

        public async Task<ICollection<T>> GetPaginatedOrderedByDateDescendingAsync<T>(
            int skip,
            int take,
            string userId = null)
        {
            var edits = this.editsRepository.All();

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

        public Task<T> GetById<T>(Guid id)
            => this.editsRepository.AllAsNoTracking()
                .Where(x => x.Id == id)
                .To<T>()
                .FirstOrDefaultAsync();

        public async Task CreateAsync(
            Guid articleId,
            string editorId,
            string articleTitle,
            string articleContent,
            string articleDescription,
            string[] articleCategories,
            string articleThumbnailLink,
            string editComment,
            (int Index, DiffOperation Operation, string Text)[] diffs)
        {
            // TODO: pls have time to optimize this...
            var inputCategories = articleCategories
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.ToLowerInvariant())
                .ToList();

            var existingCategories = await this.categoryRepository
                .AllAsNoTracking()
                .Where(x => inputCategories.Contains(x.Name))
                .ToListAsync();

            var newCategories = inputCategories
                .Except(existingCategories.Select(x => x.Name))
                .Select(x => new Category { Name = x })
                .ToList();

            var article = await this.articleRepository.All()
                .Include(x => x.Categories)
                .Where(x => x.Id == articleId)
                .FirstOrDefaultAsync();

            existingCategories.Concat(newCategories)
                .Where(x => article.Categories.All(y => y.Name != x.Name))
                .ToList()
                .ForEach(x => article.Categories.Add(x));

            article.Title = articleTitle;
            article.Content = articleContent;
            article.Description = articleDescription;
            article.ThumbnailLink = articleThumbnailLink;
            article.Edits.Add(new Edit
            {
                Comment = editComment,
                EditorId = editorId,
                Diffs = diffs
                    .Select(x => new Diff
                    {
                        Index = x.Index,
                        Operation = x.Operation,
                        Text = x.Text,
                    })
                    .ToList(),
                Notification =
                {
                    SenderId = editorId,
                },
            });

            await this.articleRepository.SaveChangesAsync();
        }
    }
}

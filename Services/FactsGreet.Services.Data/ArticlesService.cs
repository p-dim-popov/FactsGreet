﻿namespace FactsGreet.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using FactsGreet.Data.Common.Repositories;
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;
    using Microsoft.EntityFrameworkCore;

    public class ArticlesService
    {
        private readonly IDeletableEntityRepository<Article> articleRepository;
        private readonly IRepository<Category> categoryRepository;
        private readonly IRepository<Star> starRepository;
        private readonly IRepository<ArticleDeletionRequest> articleDeletionRequestRepository;

        public ArticlesService(
            IDeletableEntityRepository<Article> articleRepository,
            IRepository<Category> categoryRepository,
            IRepository<Star> starRepository,
            IRepository<ArticleDeletionRequest> articleDeletionRequestRepository)
        {
            this.articleRepository = articleRepository;
            this.categoryRepository = categoryRepository;
            this.starRepository = starRepository;
            this.articleDeletionRequestRepository = articleDeletionRequestRepository;
        }

        public Task<string> GetTitleByIdAsync(Guid id)
        {
            return this.articleRepository.All()
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
            await this.CreateNoSaveAsync(
                authorId,
                title,
                content,
                categories,
                thumbnailLink,
                description);
            await this.articleRepository.SaveChangesAsync();
        }

        public async Task<bool> ToggleStarAsync(string userId, Guid articleId)
        {
            var isStarred = await this.starRepository.All()
                .AnyAsync(x => x.UserId == userId && x.ArticleId == articleId);

            if (isStarred)
            {
                this.starRepository.Delete(new Star {ArticleId = articleId, UserId = userId});
                isStarred = false;
            }
            else
            {
                await this.starRepository.AddAsync(new Star {ArticleId = articleId, UserId = userId});
                isStarred = true;
            }

            await this.articleRepository.SaveChangesAsync();
            return isStarred;
        }

        public async Task<ICollection<T>> GetPaginatedByTitleKeywordsAsync<T>(int skip, int take, string keywords)
        {
            return await Regex.Matches(keywords, @"\""([^)]+)\""|([^\s"".?!]+)")
                .Select(x => x.Value.Replace("\"", string.Empty))
                .Aggregate(
                    this.articleRepository.All(),
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
                .Aggregate(this.articleRepository.All(), (current, keyword)
                    => current.Where(x => x.Title.Contains(keyword)))
                .CountAsync();
        }

        public async Task<ICollection<T>> GetPaginatedOrderedByDateDescendingAsync<T>(int skip, int take)
        {
            return await this.articleRepository.All()
                .OrderByDescending(x => x.ModifiedOn)
                .To<T>()
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<ICollection<T>> GetAllByTitleAsync<T>(string title)
        {
            return await this.articleRepository.All()
                .Where(x => x.Title.Contains(title))
                .To<T>()
                .ToListAsync();
        }

        public async Task<T> GetByTitleAsync<T>(string title)
        {
            return await this.articleRepository.All()
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
            return this.articleRepository.All()
                .AnyAsync(x => x.Title == title);
        }

        public async Task CreateDeletionRequestAsync(Guid id, string authorId)
        {
            await this.articleDeletionRequestRepository.AddAsync(new ArticleDeletionRequest
            {
                ArticleId = id,
                Notification = {SenderId = authorId},
            });

            await this.articleDeletionRequestRepository.SaveChangesAsync();
        }

        private async Task CreateNoSaveAsync(
            string authorId,
            string title,
            string content,
            string[] categories,
            string thumbnailLink,
            string description)
        {
            var inputCategories = categories
                .Select(x => x.ToLowerInvariant())
                .ToList();

            var existingCategories = await this.categoryRepository
                .All()
                .Where(x => inputCategories.Contains(x.Name))
                .ToListAsync();

            var newCategories = categories
                .Except(existingCategories.Select(x => x.Name))
                .Select(x => new Category {Name = x})
                .ToList();

            var article = new Article
            {
                AuthorId = authorId,
                Title = title,
                Content = content,
                ThumbnailLink = thumbnailLink,
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
                        Modifications = new List<Modification>
                        {
                            new Modification
                            {
                                Line = 0,
                                Down = string.Empty,
                                Up = content,
                            },
                        },
                    },
                },
            };

            await this.articleRepository.AddAsync(article);
        }
    }
}
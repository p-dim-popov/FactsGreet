namespace FactsGreet.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FactsGreet.Data.Common.Repositories;
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Data.Implementations;
    using FactsGreet.Services.Data.Tests.DataHelpers;
    using FactsGreet.Services.Data.Tests.Models.Articles;
    using MockQueryable.Moq;
    using Moq;
    using Xunit;

    public class ArticlesServiceTests : Tests<ArticlesService>
    {
        [Fact]
        public async Task GetTitleAsync_ShouldWorkAsExpected_WhenArticleExists()
        {
            var list = this.GetArticles().OrderBy(_ => this.Random.Next()).ToList();
            var repo = new Mock<IDeletableEntityRepository<Article>>();
            repo.Setup(r => r.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new ArticlesService(
                repo.Object,
                null,
                null,
                null,
                null,
                null);
            var expectedArticle = list
                .First();

            var actualTitle = await service.GetTitleAsync(expectedArticle.Id);

            Assert.Equal(expectedArticle.Title, actualTitle);
        }

        [Fact]
        public async Task GetTitleAsync_ShouldReturnNull_WhenArticleDoesNotExist()
        {
            var list = this.GetArticles().OrderBy(_ => this.Random.Next()).ToList();
            var repo = new Mock<IDeletableEntityRepository<Article>>();
            repo.Setup(r => r.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new ArticlesService(
                repo.Object,
                null,
                null,
                null,
                null,
                null);
            var guid = Guid.Empty;

            var actualTitle = await service.GetTitleAsync(guid);

            Assert.Null(actualTitle);
        }

        // TODO: test CreateAsync
        // TODO: test GetPaginatedByTitleKeywordsAsync
        // TODO: test GetCountByTitleKeywordsAsync
        [Fact]
        public async Task GetByTitleAsync_ShouldWorkAsExpected_WhenAllUpperCaseTitleIsPassed()
        {
            var list = this.GetArticles().OrderBy(_ => this.Random.Next()).ToList();
            var repo = new Mock<IDeletableEntityRepository<Article>>();
            repo.Setup(r => r.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new ArticlesService(
                repo.Object,
                null,
                null,
                null,
                null,
                null);
            var expectedArticle = list.First();

            var actualArticle = await service.GetByTitleAsync<ArticleWithId>(expectedArticle.Title.ToUpper());

            Assert.Equal(expectedArticle.Id, actualArticle.Id);
        }

        [Fact]
        public async Task GetByTitleAsync_ShouldReturnNull_WhenArticleNotFound()
        {
            var list = this.GetArticles().OrderBy(_ => this.Random.Next()).ToList();
            var repo = new Mock<IDeletableEntityRepository<Article>>();
            repo.Setup(r => r.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new ArticlesService(
                repo.Object,
                null,
                null,
                null,
                null,
                null);

            var actualArticle = await service.GetByTitleAsync<ArticleWithNoProperties>(string.Empty);

            Assert.Null(actualArticle);
        }

        [Fact]
        public async Task GetCountAsync_ShouldWorkAsExpected()
        {
            var list = this.GetArticles().OrderBy(_ => this.Random.Next()).ToList();
            var repo = new Mock<IDeletableEntityRepository<Article>>();
            repo.Setup(r => r.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new ArticlesService(
                repo.Object,
                null,
                null,
                null,
                null,
                null);

            var count = await service.GetCountAsync();

            Assert.Equal(list.Count, count);
        }

        [Fact]
        public async Task DoesTitleExistAsync_ShouldReturnTrue_WhenTitleExist_CaseInsensitive()
        {
            var list = this.GetArticles().OrderBy(_ => this.Random.Next()).ToList();
            var repo = new Mock<IDeletableEntityRepository<Article>>();
            repo.Setup(r => r.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new ArticlesService(
                repo.Object,
                null,
                null,
                null,
                null,
                null);
            var title = list.First().Title.ToUpper();

            var result = await service.DoesTitleExistAsync(title);

            Assert.True(result);
        }

        [Fact]
        public async Task DoesTitleExistAsync_ShouldReturnFalse_WhenTitleDoesNotExist_CaseInsensitive()
        {
            var list = this.GetArticles().OrderBy(_ => this.Random.Next()).ToList();
            var repo = new Mock<IDeletableEntityRepository<Article>>();
            repo.Setup(r => r.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new ArticlesService(
                repo.Object,
                null,
                null,
                null,
                null,
                null);

            var result = await service.DoesTitleExistAsync(string.Empty);

            Assert.False(result);
        }

        // TODO: test CreateDeletionRequestAsync
        // TODO: test DeleteAsync
        [Fact]
        public async Task GetAuthorIdAsync_ShouldWorkAsExpected_WhenArticleExists()
        {
            var list = this.GetArticles().OrderBy(_ => this.Random.Next()).ToList();
            var repo = new Mock<IDeletableEntityRepository<Article>>();
            repo.Setup(r => r.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new ArticlesService(
                repo.Object,
                null,
                null,
                null,
                null,
                null);
            var article = list.First();

            var result = await service.GetAuthorIdAsync(article.Id);

            Assert.Equal(article.AuthorId, result);
        }

        [Fact]
        public async Task GetAuthorIdAsync_ShouldReturnNull_WhenArticleDoesNotExist()
        {
            var list = this.GetArticles().OrderBy(_ => this.Random.Next()).ToList();
            var repo = new Mock<IDeletableEntityRepository<Article>>();
            repo.Setup(r => r.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new ArticlesService(
                repo.Object,
                null,
                null,
                null,
                null,
                null);

            var result = await service.GetAuthorIdAsync(Guid.Empty);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldWorkAsExpected_WhenArticleExists()
        {
            var list = this.GetArticles().OrderBy(_ => this.Random.Next()).ToList();
            var repo = new Mock<IDeletableEntityRepository<Article>>();
            repo.Setup(r => r.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new ArticlesService(
                repo.Object,
                null,
                null,
                null,
                null,
                null);
            var article = list.First();

            var result = await service.GetByIdAsync<ArticleWithId>(article.Id);

            Assert.Equal(article.Id, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenArticleDoesNotExist()
        {
            var list = this.GetArticles().OrderBy(_ => this.Random.Next()).ToList();
            var repo = new Mock<IDeletableEntityRepository<Article>>();
            repo.Setup(r => r.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new ArticlesService(
                repo.Object,
                null,
                null,
                null,
                null,
                null);

            var result = await service.GetByIdAsync<ArticleWithId>(Guid.Empty);

            Assert.Null(result);
        }
    }
}

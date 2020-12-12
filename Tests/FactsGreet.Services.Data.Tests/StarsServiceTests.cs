namespace FactsGreet.Services.Data.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FactsGreet.Data.Common.Repositories;
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Data.Implementations;
    using FactsGreet.Services.Data.Tests.DataHelpers;
    using MockQueryable.Moq;
    using Moq;
    using Xunit;

    public class StarsServiceTests : Tests<StarsService>
    {
        [Fact]
        public async Task CreateStarLinkAsync_ShouldWorkAsExpected_WhenNoDeleted()
        {
            var list = new List<Star>();
            var user = this.GetUsers().OrderBy(_ => this.Random.Next()).First();
            var article = this.GetArticles().OrderBy(_ => this.Random.Next()).First();
            var starRepo = new Mock<IDeletableEntityRepository<Star>>();
            starRepo
                .Setup(x => x.AllWithDeleted())
                .Returns(list.AsQueryable().BuildMock().Object);
            starRepo
                .Setup(x => x.AddAsync(It.IsAny<Star>()))
                .Callback((Star s) => list.Add(s));
            var service = new StarsService(starRepo.Object);

            await service.CreateStarLinkAsync(user.Id, article.Id);

            Assert.Single(list);
            Assert.False(list.First().IsDeleted);
            starRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateStarLinkAsync_ShouldMarkLinkAsNotDeleted_WhenDeleted()
        {
            var user = this.GetUsers().OrderBy(_ => this.Random.Next()).First();
            var article = this.GetArticles().OrderBy(_ => this.Random.Next()).First();
            var list = new List<Star>
            {
                new Star { UserId = user.Id, ArticleId = article.Id, IsDeleted = true },
            };
            var starRepo = new Mock<IDeletableEntityRepository<Star>>();
            starRepo
                .Setup(x => x.AllWithDeleted())
                .Returns(list.AsQueryable().BuildMock().Object);
            starRepo
                .Setup(x => x.Undelete(It.IsAny<Star>()))
                .Callback((Star s) => s.IsDeleted = false);
            var service = new StarsService(starRepo.Object);

            await service.CreateStarLinkAsync(user.Id, article.Id);

            Assert.Single(list);
            Assert.False(list.First().IsDeleted);
            starRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateStarLinkAsync_ShouldNotAddAnotherLink_WhenInvokedTwiceOnUserArticle()
        {
            var user = this.GetUsers().OrderBy(_ => this.Random.Next()).First();
            var article = this.GetArticles().OrderBy(_ => this.Random.Next()).First();
            var list = new List<Star>
            {
                new Star { UserId = user.Id, ArticleId = article.Id },
            };
            var starRepo = new Mock<IDeletableEntityRepository<Star>>();
            starRepo
                .Setup(x => x.AllWithDeleted())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new StarsService(starRepo.Object);

            await service.CreateStarLinkAsync(user.Id, article.Id);

            Assert.Single(list);
            starRepo.Verify(x => x.AddAsync(It.IsAny<Star>()), Times.Never);
            starRepo.Verify(x => x.Undelete(It.IsAny<Star>()), Times.Never);
            Assert.False(list.First().IsDeleted);
            starRepo.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task RemoveStarLinkAsync_ShouldMarkLinkAsDeleted_WhenLinkExists()
        {
            var user = this.GetUsers().OrderBy(_ => this.Random.Next()).First();
            var article = this.GetArticles().OrderBy(_ => this.Random.Next()).First();
            var list = new List<Star>
            {
                new Star { UserId = user.Id, ArticleId = article.Id },
            };
            var starRepo = new Mock<IDeletableEntityRepository<Star>>();
            starRepo
                .Setup(x => x.All())
                .Returns(list.AsQueryable().BuildMock().Object);
            starRepo
                .Setup(x => x.Delete(It.IsAny<Star>()))
                .Callback((Star s) => s.IsDeleted = true);
            var service = new StarsService(starRepo.Object);

            await service.RemoveStarLinkAsync(user.Id, article.Id);

            Assert.Single(list);
            Assert.True(list.First().IsDeleted);
            starRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RemoveStarLinkAsync_ShouldMarkLinkAsDeleted_WhenLinkDoesNotExist()
        {
            var user = this.GetUsers().OrderBy(_ => this.Random.Next()).First();
            var article = this.GetArticles().OrderBy(_ => this.Random.Next()).First();
            var list = new List<Star>();
            var starRepo = new Mock<IDeletableEntityRepository<Star>>();
            starRepo
                .Setup(x => x.All())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new StarsService(starRepo.Object);

            await service.RemoveStarLinkAsync(user.Id, article.Id);

            Assert.Empty(list);
            starRepo.Verify(x => x.Delete(It.IsAny<Star>()), Times.Never);
            starRepo.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task IsArticleStarredByUserAsync_ShouldReturnTrue_WhenArticleIsStarred()
        {
            var user = this.GetUsers().OrderBy(_ => this.Random.Next()).First();
            var article = this.GetArticles().OrderBy(_ => this.Random.Next()).First();
            var list = new List<Star>
            {
                new Star { ArticleId = article.Id, UserId = user.Id },
            };
            var starRepo = new Mock<IDeletableEntityRepository<Star>>();
            starRepo
                .Setup(x => x.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new StarsService(starRepo.Object);

            var result = await service.IsArticleStarredByUserAsync(article.Id, user.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task IsArticleStarredByUserAsync_ShouldReturnFalse_WhenArticleIsNotStarred()
        {
            var user = this.GetUsers().OrderBy(_ => this.Random.Next()).First();
            var article = this.GetArticles().OrderBy(_ => this.Random.Next()).First();
            var starRepo = new Mock<IDeletableEntityRepository<Star>>();
            starRepo
                .Setup(x => x.AllAsNoTracking())
                .Returns(new List<Star>().AsQueryable().BuildMock().Object);
            var service = new StarsService(starRepo.Object);

            var result = await service.IsArticleStarredByUserAsync(article.Id, user.Id);

            Assert.False(result);
        }
    }
}

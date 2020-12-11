namespace FactsGreet.Services.Data.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using FactsGreet.Data.Common.Repositories;
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Data.Implementations;
    using FactsGreet.Services.Data.Tests.DataHelpers;
    using FactsGreet.Services.Data.Tests.Models.ApplicationUsers;
    using MockQueryable.Moq;
    using Moq;
    using Xunit;

    public class ApplicationUsersServiceTests : Tests<ApplicationUsersService>
    {
        [Fact]
        public async Task GetByEmailAsync_ShouldWorkAsExpected_WhenNotNormalizedEmailIsPassed()
        {
            var list = this.GetUsers().OrderBy(_ => this.Random.Next()).ToList();
            var repo = new Mock<IDeletableEntityRepository<ApplicationUser>>();
            repo.Setup(r => r.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new ApplicationUsersService(repo.Object);
            var expectedUser = list
                .OrderBy(_ => this.Random.Next())
                .First(x => x.NormalizedEmail != x.Email);

            var actualUser = await service.GetByEmailAsync<ApplicationUserWithEmail>(expectedUser.Email);

            Assert.Equal(expectedUser.NormalizedEmail, actualUser.NormalizedEmail);
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldReturnNull_WhenNoUserIsFound()
        {
            var list = this.GetUsers().OrderBy(_ => this.Random.Next()).ToList();
            var repo = new Mock<IDeletableEntityRepository<ApplicationUser>>();
            repo.Setup(r => r.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new ApplicationUsersService(repo.Object);

            var actualUser = await service.GetByEmailAsync<ApplicationUserWithEmail>(string.Empty);

            Assert.Null(actualUser);
        }

        [Fact]
        public async Task GetEmailAsync_ShouldWorkAsExpected()
        {
            var list = this.GetUsers().OrderBy(_ => this.Random.Next()).ToList();
            var repo = new Mock<IDeletableEntityRepository<ApplicationUser>>();
            repo.Setup(r => r.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new ApplicationUsersService(repo.Object);
            var expectedUser = list
                .OrderBy(_ => this.Random.Next())
                .First();

            var actualEmail = await service.GetEmailAsync(expectedUser.Id);

            Assert.Equal(expectedUser.Email, actualEmail);
        }

        [Fact]
        public async Task GetEmailAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            var list = this.GetUsers().OrderBy(_ => this.Random.Next()).ToList();
            var repo = new Mock<IDeletableEntityRepository<ApplicationUser>>();
            repo.Setup(r => r.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new ApplicationUsersService(repo.Object);

            var actualEmail = await service.GetEmailAsync(string.Empty);

            Assert.Null(actualEmail);
        }

        [Fact]
        public async Task GetIdByEmailAsync_ShouldWorkAsExpected_WhenNotNormalizedEmailIsPassed()
        {
            var list = this.GetUsers().OrderBy(_ => this.Random.Next()).ToList();
            var repo = new Mock<IDeletableEntityRepository<ApplicationUser>>();
            repo.Setup(r => r.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new ApplicationUsersService(repo.Object);
            var expectedUser = list
                .OrderBy(_ => this.Random.Next())
                .First(x => x.NormalizedEmail != x.Email);

            var actualId = await service.GetIdByEmailAsync(expectedUser.Email);

            Assert.Equal(expectedUser.Id, actualId);
        }

        [Fact]
        public async Task GetIdByEmailAsync_ShouldReturnNull_WhenNoUserIsFound()
        {
            var list = this.GetUsers().OrderBy(_ => this.Random.Next()).ToList();
            var repo = new Mock<IDeletableEntityRepository<ApplicationUser>>();
            repo.Setup(r => r.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new ApplicationUsersService(repo.Object);

            var actualId = await service.GetIdByEmailAsync(string.Empty);

            Assert.Null(actualId);
        }

        [Fact]
        public async Task Get10EmailsByEmailKeywordAsync_ShouldReturnUpTo10Emails_WhenEmailContainsKeyword()
        {
            var list = this.GetJinAndJonesAtHome().OrderBy(_ => this.Random.Next()).ToList();
            var repo = new Mock<IDeletableEntityRepository<ApplicationUser>>();
            repo.Setup(r => r.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new ApplicationUsersService(repo.Object);

            var result = await service.Get10EmailsByEmailKeywordAsync("in");

            Assert.True(result.Count <= 10);
            Assert.All(result, x => Assert.Contains("in", x.ToLower()));
        }

        // TODO: Get10MostActiveUsersForTheLastWeekAsync
    }
}

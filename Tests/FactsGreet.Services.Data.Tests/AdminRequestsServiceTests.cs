namespace FactsGreet.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FactsGreet.Data.Common.Repositories;
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Data.Implementations;
    using FactsGreet.Services.Data.Tests.DataHelpers;
    using MockQueryable.Moq;
    using Moq;
    using Xunit;

    public class AdminRequestsServiceTests : Tests<AdminRequestsService>
    {
        [Fact]
        public async Task FollowAsync_ShouldWorkAsExpected_WhenUserIsFollowingUserForTheFirstTime()
        {
            var list = this.GetRequests().OrderBy(_ => this.Random.Next()).ToList();
            var repo = new Mock<IDeletableEntityRepository<AdminRequest>>();
            repo.Setup(r => r.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new AdminRequestsService(repo.Object, null);

            var result = await service.GetCountAsync();

            Assert.Equal(list.Count, result);
        }

        [Fact]
        public async Task CanUserBecomeAdminAsync_ShouldReturnTrue_WhenHasHalfTheCountOfBadges()
        {
            var badges = BadgesServiceDataHelper.GetBadges(null);
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                Badges = badges.Take((badges.Count / 2) + 1).ToList(),
            };
            var mock = new Mock<IBadgesService>();
            mock.Setup(m => m.GetCountAsync())
                .ReturnsAsync(badges.Count);
            var guidTest = Guid.Empty;
            mock.Setup(m => m.GetUserBadgesCountByUserIdAsync(It.Is<string>(x => Guid.TryParse(x, out guidTest))))
                .ReturnsAsync(user.Badges.Count);
            var service = new AdminRequestsService(null, mock.Object);

            var result = await service.CanUserBecomeAdminAsync(user.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task CanUserBecomeAdminAsync_ShouldReturnFalse_WhenDoesNotHasHalfTheCountOfBadges()
        {
            var badges = BadgesServiceDataHelper.GetBadges(null);
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                Badges = badges.Take((badges.Count / 2) - 1).ToList(),
            };
            var mock = new Mock<IBadgesService>();
            mock.Setup(m => m.GetCountAsync())
                .ReturnsAsync(badges.Count);
            var guidTest = Guid.Empty;
            mock.Setup(m => m.GetUserBadgesCountByUserIdAsync(It.Is<string>(x => Guid.TryParse(x, out guidTest))))
                .ReturnsAsync(user.Badges.Count);
            var service = new AdminRequestsService(null, mock.Object);

            var result = await service.CanUserBecomeAdminAsync(user.Id);

            Assert.False(result);
        }
    }
}

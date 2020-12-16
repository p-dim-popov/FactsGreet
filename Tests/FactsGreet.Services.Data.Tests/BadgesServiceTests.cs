namespace FactsGreet.Services.Data.Tests
{
    using System;
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

    public class BadgesServiceTests : Tests<BadgesService>
    {
        [Fact]
        public async Task GetAllAsync_ShouldWorkAsExpected()
        {
            var list = this.GetBadges().OrderBy(_ => this.Random.Next()).ToList();
            var repo = new Mock<IRepository<Badge>>();
            repo.Setup(r => r.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new BadgesService(null, repo.Object);

            var result = await service.GetAllAsync();

            Assert.Equal(list.Count, result.Count);
            Assert.All(result, x => Assert.Contains(x, list));
        }

        [Fact]
        public async Task RemoveBadgeFromUserAsync_ShouldWorkAsExpected_WhenUserHasTheBadge()
        {
            var (users, _) = this.GetUsersWithBadges();
            var userRepo = new Mock<IDeletableEntityRepository<ApplicationUser>>();
            userRepo.Setup(r => r.All())
                .Returns(users.AsQueryable().BuildMock().Object);
            var service = new BadgesService(userRepo.Object, null);
            var user = users.First();
            var badgeName = user.Badges.First().Name;

            await service.RemoveBadgeFromUserAsync(user.Id, badgeName);

            userRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.DoesNotContain(user.Badges, x => x.Name == badgeName);
        }

        [Fact]
        public async Task RemoveBadgeFromUserAsync_ShouldNotRemoveAnything_WhenUserDoesNotHasTheBadge()
        {
            var (users, badges) = this.GetUsersWithBadges();
            var userRepo = new Mock<IDeletableEntityRepository<ApplicationUser>>();
            userRepo.Setup(r => r.All())
                .Returns(users.AsQueryable().BuildMock().Object);
            var service = new BadgesService(userRepo.Object, null);
            var user = users.First(x => x.Badges.Count < badges.Count);
            var userBadgesCount = user.Badges.Count;
            var badgeName = badges.First(x => !user.Badges.Contains(x)).Name;

            await service.RemoveBadgeFromUserAsync(user.Id, badgeName);

            userRepo.Verify(x => x.SaveChangesAsync(), Times.Never);
            Assert.Equal(userBadgesCount, user.Badges.Count);
            Assert.DoesNotContain(user.Badges, x => x.Name == badgeName);
        }

        [Fact]
        public async Task AddBadgeToUserAsync_ShouldWorkAsExpected_WhenUserDoesNotHasTheBadge()
        {
            var (users, badges) = this.GetUsersWithBadges();
            var userRepo = new Mock<IDeletableEntityRepository<ApplicationUser>>();
            userRepo.Setup(r => r.All())
                .Returns(users.AsQueryable().BuildMock().Object);
            var badgesRepo = new Mock<IRepository<Badge>>();
            badgesRepo.Setup(r => r.All())
                .Returns(badges.AsQueryable().BuildMock().Object);
            var service = new BadgesService(userRepo.Object, badgesRepo.Object);
            var user = users.First(x => x.Badges.Count < badges.Count);
            var userBadgesCount = user.Badges.Count;
            var badgeName = badges.First(x => !user.Badges.Contains(x)).Name;

            await service.AddBadgeToUserAsync(user.Id, badgeName);

            userRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.Equal(userBadgesCount + 1, user.Badges.Count);
            Assert.Contains(user.Badges, x => x.Name == badgeName);
        }

        [Fact]
        public async Task AddBadgeToUserAsync_ShouldNotAddAnotherOne_WhenUserHasTheBadge()
        {
            var (users, badges) = this.GetUsersWithBadges();
            var userRepo = new Mock<IDeletableEntityRepository<ApplicationUser>>();
            userRepo.Setup(r => r.All())
                .Returns(users.AsQueryable().BuildMock().Object);
            var badgesRepo = new Mock<IRepository<Badge>>();
            badgesRepo.Setup(r => r.All())
                .Returns(badges.AsQueryable().BuildMock().Object);
            var service = new BadgesService(userRepo.Object, badgesRepo.Object);
            var user = users.First();
            var userBadgesCount = user.Badges.Count;
            var badgeName = user.Badges.First().Name;

            await service.AddBadgeToUserAsync(user.Id, badgeName);

            userRepo.Verify(x => x.SaveChangesAsync(), Times.Never);
            Assert.Equal(userBadgesCount, user.Badges.Count);
            Assert.Contains(user.Badges, x => x.Name == badgeName);
        }

        [Fact]
        public async Task AddBadgeToUserAsync_ShouldNotAddOne_WhenBadgeDoesNotExist()
        {
            var (users, badges) = this.GetUsersWithBadges();
            var userRepo = new Mock<IDeletableEntityRepository<ApplicationUser>>();
            userRepo.Setup(r => r.All())
                .Returns(users.AsQueryable().BuildMock().Object);
            var badgesRepo = new Mock<IRepository<Badge>>();
            badgesRepo.Setup(r => r.All())
                .Returns(badges.AsQueryable().BuildMock().Object);
            var service = new BadgesService(userRepo.Object, badgesRepo.Object);
            var user = users.First();
            var userBadgesCount = user.Badges.Count;
            var badgeName = string.Empty;

            await service.AddBadgeToUserAsync(user.Id, badgeName);

            userRepo.Verify(x => x.SaveChangesAsync(), Times.Never);
            Assert.Equal(userBadgesCount, user.Badges.Count);
            Assert.DoesNotContain(user.Badges, x => x.Name == badgeName);
        }

        [Fact]
        public async Task GetCountAsync_ShouldWorkAsExpected()
        {
            var list = this.GetBadges().ToList();
            var repo = new Mock<IRepository<Badge>>();
            repo.Setup(r => r.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new BadgesService(null, repo.Object);

            var result = await service.GetCountAsync();

            Assert.Equal(list.Count, result);
        }

        [Fact]
        public async Task GetUserBadgesCountByUserIdAsync_ShouldWorkAsExpected()
        {
            var badges = this.GetBadges();
            var list = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Badges = badges.Take(this.Random.Next(0, badges.Count)).ToList(),
                },
            };
            var repo = new Mock<IDeletableEntityRepository<ApplicationUser>>();
            repo.Setup(r => r.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new BadgesService(repo.Object, null);
            var user = list.First();

            var result = await service.GetUserBadgesCountByUserIdAsync(user.Id);

            Assert.Equal(user.Badges.Count, result);
        }
    }
}

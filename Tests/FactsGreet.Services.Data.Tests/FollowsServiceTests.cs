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
    using FactsGreet.Services.Data.Tests.Models.ApplicationUsers;
    using MockQueryable.Moq;
    using Moq;
    using Xunit;

    public class FollowsServiceTests : Tests<FollowsService>
    {
        [Fact]
        public async Task FollowAsync_ShouldWorkAsExpected_WhenUserIsFollowingUserForTheFirstTime()
        {
            var users = this.GetFollowedUsers().OrderBy(_ => this.Random.Next()).ToList();
            var follows = users
                .SelectMany(x => x.Followers)
                .Concat(users
                    .SelectMany(x => x.Followings))
                .ToList();
            var repo = new Mock<IDeletableEntityRepository<Follow>>();
            repo.Setup(r => r.AllWithDeleted())
                .Returns(follows.AsQueryable().BuildMock().Object);
            repo.Setup(r => r.AddAsync(It.IsAny<Follow>()))
                .Callback((Follow f) => follows.Add(f));
            var service = new FollowsService(repo.Object);
            var follower = users.First();
            var followed = users.First(x => x
                .Followers.All(y => y.FollowerId != follower.Id));
            var followsCount = follows.Count;

            await service.FollowAsync(follower.Id, followed.Id);

            repo.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.Equal(followsCount + 1, follows.Count);
            Assert.Contains(follows, x => x.FollowerId == follower.Id && x.FollowedId == followed.Id);
        }

        [Fact]
        public async Task FollowAsync_ShouldChange_IsDeleted_ToTrue_WhenUserStartsFollowingUserAfterUnfollowing()
        {
            var users = this.GetFollowedUsers().OrderBy(_ => this.Random.Next()).ToList();
            var follows = users
                .SelectMany(x => x.Followers)
                .Concat(users
                    .SelectMany(x => x.Followings))
                .ToList();
            var repo = new Mock<IDeletableEntityRepository<Follow>>();
            repo.Setup(r => r.AllWithDeleted())
                .Returns(follows.AsQueryable().BuildMock().Object);
            repo.Setup(r => r.Undelete(It.IsAny<Follow>()))
                .Callback((Follow f) => f.IsDeleted = false);
            var service = new FollowsService(repo.Object);
            var follower = users.First();
            var followed = users.First(x => x.Id == follower.Followings.First().FollowedId);
            var follow = follows.First(x => x.FollowerId == follower.Id && x.FollowedId == followed.Id);
            follow.IsDeleted = true;
            var followsCount = follows.Count;

            await service.FollowAsync(follower.Id, followed.Id);

            repo.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.Equal(followsCount, follows.Count);
            Assert.False(follow.IsDeleted);
        }

        [Fact]
        public async Task FollowAsync_ShouldNotAddAnotherFollow_WhenUserFollowsUserTwice()
        {
            var users = this.GetFollowedUsers().OrderBy(_ => this.Random.Next()).ToList();
            var follows = users
                .SelectMany(x => x.Followers)
                .Concat(users
                    .SelectMany(x => x.Followings))
                .ToList();
            var repo = new Mock<IDeletableEntityRepository<Follow>>();
            repo.Setup(r => r.AllWithDeleted())
                .Returns(follows.AsQueryable().BuildMock().Object);
            repo.Setup(r => r.Undelete(It.IsAny<Follow>()))
                .Callback((Follow f) => f.IsDeleted = false);
            var service = new FollowsService(repo.Object);
            var follower = users.First();
            var followed = users.First(x => x.Id == follower.Followings.First().FollowedId);

            var followsCount = follows.Count;

            await service.FollowAsync(follower.Id, followed.Id);

            repo.Verify(x => x.SaveChangesAsync(), Times.Never);
            Assert.Equal(followsCount, follows.Count);
        }

        [Fact]
        public async Task UnfollowAsync_ShouldWorkAsExpected_WhenUserIsFollowingUser()
        {
            var users = this.GetFollowedUsers().OrderBy(_ => this.Random.Next()).ToList();
            var follows = users
                .SelectMany(x => x.Followers)
                .Concat(users
                    .SelectMany(x => x.Followings))
                .ToList();
            var repo = new Mock<IDeletableEntityRepository<Follow>>();
            repo.Setup(r => r.All())
                .Returns(follows.AsQueryable().BuildMock().Object);
            repo.Setup(r => r.Delete(It.IsAny<Follow>()))
                .Callback((Follow f) => follows.Remove(f));
            var service = new FollowsService(repo.Object);
            var follower = users.First();
            var followed = users.First(x => x.Id == follower.Followings.First().FollowedId);

            var followsCount = follows.Count;

            await service.UnfollowAsync(follower.Id, followed.Id);

            repo.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.Equal(followsCount - 1, follows.Count);
            Assert.DoesNotContain(follows, x => x.FollowerId == follower.Id && x.FollowedId == followed.Id);
        }

        [Fact]
        public async Task UnfollowAsync_ShouldDoNothing_WhenUserIsNotFollowingUser()
        {
            var users = this.GetFollowedUsers().OrderBy(_ => this.Random.Next()).ToList();
            var follows = users
                .SelectMany(x => x.Followers)
                .Concat(users
                    .SelectMany(x => x.Followings))
                .ToList();
            var repo = new Mock<IDeletableEntityRepository<Follow>>();
            repo.Setup(r => r.All())
                .Returns(follows.AsQueryable().BuildMock().Object);
            var service = new FollowsService(repo.Object);
            var follower = users.First();
            var followed = users.First(x => x.Id != follower.Followings.First().FollowedId);

            var followsCount = follows.Count;

            await service.UnfollowAsync(follower.Id, followed.Id);

            repo.Verify(x => x.SaveChangesAsync(), Times.Never);
            Assert.Equal(followsCount, follows.Count);
            Assert.DoesNotContain(follows, x => x.FollowerId == follower.Id && x.FollowedId == followed.Id);
        }

        [Fact]
        public async Task IsUserFollowingUserAsync_ShouldReturnTrue_WhenUserIsFollowingUser()
        {
            var users = this.GetFollowedUsers().OrderBy(_ => this.Random.Next()).ToList();
            var follows = users
                .SelectMany(x => x.Followers)
                .Concat(users
                    .SelectMany(x => x.Followings))
                .ToList();
            var repo = new Mock<IDeletableEntityRepository<Follow>>();
            repo.Setup(r => r.AllAsNoTracking())
                .Returns(follows.AsQueryable().BuildMock().Object);
            var service = new FollowsService(repo.Object);
            var follower = users.First();
            var followed = users.First(x => x.Id == follower.Followings.First().FollowedId);

            var result = await service.IsUserFollowingUserAsync(follower.Id, followed.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task IsUserFollowingUserAsync_ShouldReturnFalse_WhenUserIsNotFollowingUser()
        {
            var users = this.GetFollowedUsers().OrderBy(_ => this.Random.Next()).ToList();
            var follows = users
                .SelectMany(x => x.Followers)
                .Concat(users
                    .SelectMany(x => x.Followings))
                .ToList();
            var repo = new Mock<IDeletableEntityRepository<Follow>>();
            repo.Setup(r => r.AllAsNoTracking())
                .Returns(follows.AsQueryable().BuildMock().Object);
            var service = new FollowsService(repo.Object);
            var follower = users.First();
            var followed = users.First(x => x.Id != follower.Followings.First().FollowedId);

            var result = await service.IsUserFollowingUserAsync(follower.Id, followed.Id);

            Assert.False(result);
        }

        [Fact]
        public async Task GetFollowedUsersAsync_ShouldWorkAsExpected()
        {
            var follower = new ApplicationUser { Id = Guid.NewGuid().ToString(), };
            var followedUsers = Enumerable.Range(1, 10)
                .Select(x => new ApplicationUser { Id = x.ToString() });
            var follows = followedUsers
                .Select(x => new Follow { FollowerId = follower.Id, Followed = x,  })
                .ToList();
            var repo = new Mock<IDeletableEntityRepository<Follow>>();
            repo.Setup(r => r.AllAsNoTracking())
                .Returns(follows.AsQueryable().BuildMock().Object);
            var service = new FollowsService(repo.Object);

            var result = await service
                .GetFollowedUsersAsync<ApplicationUserWithId>(follower.Id);

            Assert.Equal(follows.Count, result.Count);
            Assert.True(result.All(x => follows.Any(y => y.Followed.Id == x.Id)));
        }
    }
}

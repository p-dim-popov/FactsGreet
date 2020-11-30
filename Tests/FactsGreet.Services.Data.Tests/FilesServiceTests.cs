namespace FactsGreet.Services.Data.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using FactsGreet.Common;
    using FactsGreet.Data.Common.Repositories;
    using FactsGreet.Services.Data.Implementations;
    using FactsGreet.Services.Data.Tests.DataHelpers;
    using FactsGreet.Services.Data.Tests.Models.Files;
    using FactsGreet.Web.ViewModels.Files;
    using MockQueryable.Moq;
    using Moq;
    using Xunit;
    using File = FactsGreet.Data.Models.File;

    public class FilesServiceTests : Tests<FilesService>
    {
        [Fact]
        public async Task UploadAsync_ShouldWorkAsExpected_WhenUserHasSpaceForTheFile()
        {
            var list = this.GetFiles().OrderBy(_ => this.Random.Next()).ToList();
            var fileRepo = new Mock<IDeletableEntityRepository<File>>();
            fileRepo
                .Setup(x => x.AddAsync(It.IsAny<File>()))
                .Callback((File f) => list.Add(f));
            fileRepo
                .Setup(x => x.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            const string userId = "Pesho";
            const string filename = "extremely_new_file.jpg";
            var guid = Guid.Empty;
            var dropboxService = new Mock<IDropboxService>();
            dropboxService
                .Setup(x => x.UploadAsync(It.IsAny<Guid>(), It.IsAny<Stream>()))
                .Callback((Guid g, Stream _) => guid = g)
                .ReturnsAsync((Guid g, Stream _) => g.ToString());
            var service = new FilesService(dropboxService.Object, fileRepo.Object);
            var fileSource = new byte[FilesService.MaxStorage - 1];
            await using var fileSourceStream = new MemoryStream(fileSource);

            var link = await service.UploadAsync(fileSourceStream, filename, userId);

            Assert.Equal(guid.ToString(), link);
            dropboxService.Verify(
                x => x.UploadAsync(
                    It.Is<Guid>(y => y == guid),
                    It.Is<Stream>(y => ReferenceEquals(y, fileSourceStream))),
                Times.Once);
            fileRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
            var uploadedFile = list.FirstOrDefault(x => x.Filename == filename && x.UserId == userId);
            Assert.NotNull(uploadedFile);
            Assert.Equal(guid, uploadedFile.Id);
            Assert.Equal(filename, uploadedFile.Filename);
            Assert.Equal(guid.ToString(), uploadedFile.Link);
            Assert.Equal(userId, uploadedFile.UserId);
            Assert.Equal(FilesService.MaxStorage - 1, uploadedFile.Size);
        }

        [Fact]
        public async Task UploadAsync_ShouldThrowInvalidOperationException_WhenUserHasNoSpaceForTheFile()
        {
            var list = this.GetFiles().OrderBy(_ => this.Random.Next()).ToList();
            var fileRepo = new Mock<IDeletableEntityRepository<File>>();
            fileRepo
                .Setup(x => x.AddAsync(It.IsAny<File>()))
                .Callback((File f) => list.Add(f));
            fileRepo
                .Setup(x => x.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var userId = list.Select(x => x.UserId).First();
            var dropboxService = new Mock<IDropboxService>();
            var service = new FilesService(dropboxService.Object, fileRepo.Object);
            var fileSource = new byte[FilesService.MaxStorage];
            await using var fileSourceStream = new MemoryStream(fileSource);

            var exception =
                await Assert.ThrowsAsync<InvalidOperationException>(() =>
                    service.UploadAsync(fileSourceStream, $"{nameof(userId)}'s file", userId));
            Assert.Contains("User has reached his storage capacity!", exception.Message);
            dropboxService.Verify(
                x => x.UploadAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Stream>()),
                Times.Never);
            fileRepo.Verify(x => x.AddAsync(It.IsAny<File>()), Times.Never);
            fileRepo.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task GetCountAsync_ShouldWorkAsExpected()
        {
            const int filesCount = 3;
            var fileRepo = new Mock<IDeletableEntityRepository<File>>();
            fileRepo
                .Setup(x => x.AllAsNoTracking())
                .Returns(this.GetFiles(filesCount).AsQueryable().BuildMock().Object);
            var service = new FilesService(null, fileRepo.Object);

            Assert.Equal(filesCount, await service.GetCountAsync());
        }

        [Fact]
        public async Task GetAllForUser_ShouldWorkAsExpected()
        {
            var list = this.GetFiles().OrderBy(_ => this.Random.Next()).ToList();
            var secondFileForUser = list.Last().Clone();
            secondFileForUser.Filename = "another";
            list.Add(secondFileForUser);
            var fileRepo = new Mock<IDeletableEntityRepository<File>>();
            fileRepo
                .Setup(x => x.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new FilesService(null, fileRepo.Object);

            var files = await service
                .GetAllForUserAsync<FileWithUserIdModel>(secondFileForUser.UserId);
            Assert.Equal(2, files.Count);
            Assert.All(files, x => Assert.Equal(x.UserId, secondFileForUser.UserId));
        }

        [Fact]
        public async Task IsFilenameAvailable_ShouldReturnFalse_WhenNotAvailableForUser()
        {
            var list = this.GetFiles().OrderBy(_ => this.Random.Next()).ToList();
            var fileRepo = new Mock<IDeletableEntityRepository<File>>();
            fileRepo
                .Setup(x => x.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new FilesService(null, fileRepo.Object);
            var file = list.First();

            Assert.False(await service.IsFilenameAvailableAsync(file.Filename, file.UserId));
        }

        [Fact]
        public async Task IsFilenameAvailable_ShouldReturnTrue_WhenAvailableForUser()
        {
            var list = this.GetFiles().OrderBy(_ => this.Random.Next()).ToList();
            var fileRepo = new Mock<IDeletableEntityRepository<File>>();
            fileRepo
                .Setup(x => x.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new FilesService(null, fileRepo.Object);
            var newFile = list.First().Clone();
            newFile.Filename = "really different name, also very invalid";

            Assert.True(await service.IsFilenameAvailableAsync(newFile.Filename, newFile.UserId));
        }

        [Fact]
        public async Task IsFilenameAvailable_ShouldReturnTrue_WhenSomeoneElseHaveSameFilename()
        {
            var list = this.GetFiles().OrderBy(_ => this.Random.Next()).ToList();
            var fileRepo = new Mock<IDeletableEntityRepository<File>>();
            fileRepo
                .Setup(x => x.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new FilesService(null, fileRepo.Object);
            var newFile = list.First().Clone();
            var file = list.Last();
            newFile.Filename = file.Filename;

            Assert.True(await service.IsFilenameAvailableAsync(newFile.Filename, newFile.UserId));
        }

        [Fact]
        public async Task RenameAsync_ShouldWorkAsExpected()
        {
            var list = this.GetFiles().OrderBy(_ => this.Random.Next()).ToList();
            var fileRepo = new Mock<IDeletableEntityRepository<File>>();
            fileRepo
                .Setup(x => x.All())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new FilesService(null, fileRepo.Object);
            var file = list.First();

            await service.RenameAsync(file.Id, nameof(file));

            fileRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.Equal(nameof(file), file.Filename);
        }

        [Fact]
        public async Task GetUserIdAsync_ShouldReturnUserId_WhenCorrectFileIdIsPassed()
        {
            var list = this.GetFiles().OrderBy(_ => this.Random.Next()).ToList();
            var fileRepo = new Mock<IDeletableEntityRepository<File>>();
            fileRepo
                .Setup(x => x.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new FilesService(null, fileRepo.Object);
            var file = list.First();

            Assert.Equal(file.UserId, await service.GetUserIdAsync(file.Id));
        }

        [Fact]
        public async Task GetUserIdAsync_ShouldReturnNull_WhenIncorrectFileIdIsPassed()
        {
            var list = this.GetFiles().OrderBy(_ => this.Random.Next()).ToList();
            var fileRepo = new Mock<IDeletableEntityRepository<File>>();
            fileRepo
                .Setup(x => x.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new FilesService(null, fileRepo.Object);

            Assert.Null(await service.GetUserIdAsync(Guid.Empty));
        }

        [Fact]
        public async Task DeleteAsync_ShouldWorkAsExpected()
        {
            var list = this.GetFiles().OrderBy(_ => this.Random.Next()).ToList();
            var fileRepo = new Mock<IDeletableEntityRepository<File>>();
            fileRepo
                .Setup(x => x.All())
                .Returns(list.AsQueryable().BuildMock().Object);
            fileRepo
                .Setup(x => x.HardDelete(It.IsAny<File>()))
                .Callback((File f) => list.Remove(f));
            var dropboxService = new Mock<IDropboxService>();
            var service = new FilesService(dropboxService.Object, fileRepo.Object);
            var file = list.First();

            await service.DeleteAsync(file.Id);

            dropboxService.Verify(
                x => x.DeleteAsync(It.Is<Guid>(y => y == file.Id)),
                Times.Once);
            fileRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.DoesNotContain(list, x => x.Id == file.Id);
        }

        [Fact]
        public async Task GetUsedSizeAsync_ShouldWorkAsExpected()
        {
            var list = this.GetFiles().OrderBy(_ => this.Random.Next()).ToList();
            var file = list.First().Clone();
            file.Size++;
            list.Add(file);
            var fileRepo = new Mock<IDeletableEntityRepository<File>>();
            fileRepo
                .Setup(x => x.AllAsNoTracking())
                .Returns(list.AsQueryable().BuildMock().Object);
            var service = new FilesService(null, fileRepo.Object);

            var size = await service.GetUsedSizeAsync(file.UserId);

            Assert.Equal(file.Size-- + file.Size, size);
        }
    }
}

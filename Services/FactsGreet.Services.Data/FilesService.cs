namespace FactsGreet.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Dropbox.Api;
    using Dropbox.Api.Files;
    using Dropbox.Api.Sharing;
    using FactsGreet.Data.Common.Repositories;
    using FactsGreet.Services.Mapping;
    using Microsoft.EntityFrameworkCore;

    using File = FactsGreet.Data.Models.File;

    public class FilesService
    {
        private const long MaxStorage = 100 * 1024 * 1024;
        private readonly DropboxClient dropboxClient;
        private readonly IDeletableEntityRepository<File> fileRepository;

        public FilesService(
            DropboxClient dropboxClient,
            IDeletableEntityRepository<File> fileRepository)
        {
            this.dropboxClient = dropboxClient;
            this.fileRepository = fileRepository;
        }

        public async Task<string> UploadAsync(Stream stream, long filesize, string filename, string userId)
        {
            if (await this.fileRepository.AllAsNoTracking()
                .Where(x => x.UserId == userId)
                .SumAsync(x => x.Size) + filesize > MaxStorage)
            {
                throw new InvalidOperationException("User has reached his storage capacity!");
            }

            var guid = Guid.NewGuid();
            var filename0 = "/" + guid;
            await this.dropboxClient.Files.UploadAsync(filename0, WriteMode.Overwrite.Instance, body: stream);
            var sharedLinkMetadata = await this.dropboxClient.Sharing
                .CreateSharedLinkWithSettingsAsync(
                    filename0,
                    new SharedLinkSettings(RequestedVisibility.Public.Instance));

            var file = new File
            {
                Id = guid,
                Filename = filename,
                Link = sharedLinkMetadata.Url.Replace("dl=0", "dl=1"),
                UserId = userId,
                Size = filesize,
            };

            await this.fileRepository.AddAsync(file);
            await this.fileRepository.SaveChangesAsync();

            return file.Link;
        }

        public Task<int> GetCount()
            => this.fileRepository.AllAsNoTracking()
                .CountAsync();

        public async Task<ICollection<T>> GetAllForUser<T>(string userId)
            => await this.fileRepository.AllAsNoTracking()
                .Where(x => x.UserId == userId)
                .To<T>()
                .ToListAsync();

        public Task<bool> IsFilenameAvailable(string filename, string userId)
            => this.fileRepository.AllAsNoTracking()
                .AllAsync(x => x.Filename != filename || x.UserId != userId);

        public async Task RenameAsync(Guid id, string filename)
        {
            var file = await this.fileRepository.All()
                .FirstOrDefaultAsync(x => x.Id == id);
            file.Filename = filename;
            await this.fileRepository.SaveChangesAsync();
        }

        public Task<string> GetUserId(Guid id)
            => this.fileRepository.AllAsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => x.UserId)
                .FirstOrDefaultAsync();

        public async Task DeleteAsync(Guid id)
        {
            var file = await this.fileRepository.All()
                .FirstOrDefaultAsync(x => x.Id == id);

            await this.dropboxClient.Files.DeleteV2Async("/" + file.Id);

            this.fileRepository.Delete(file);
            await this.fileRepository.SaveChangesAsync();
        }

        public Task<long> GetUsedSize(string userId)
            => this.fileRepository.AllAsNoTracking()
                .Where(x => x.UserId == userId)
                .SumAsync(x => x.Size);
    }
}

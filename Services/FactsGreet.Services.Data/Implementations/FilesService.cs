namespace FactsGreet.Services.Data.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using FactsGreet.Data.Common.Repositories;
    using FactsGreet.Services.Mapping;
    using Microsoft.EntityFrameworkCore;

    using File = FactsGreet.Data.Models.File;

    public class FilesService : IFilesService
    {
        public const long MaxStorage = 100 * 1024 * 1024;
        private readonly IDropboxService dropboxService;
        private readonly IDeletableEntityRepository<File> fileRepository;

        public FilesService(
            IDropboxService dropboxService,
            IDeletableEntityRepository<File> fileRepository)
        {
            this.dropboxService = dropboxService;
            this.fileRepository = fileRepository;
        }

        public async Task<string> UploadAsync(Stream stream, string filename, string userId)
        {
            if (await this.GetUsedSizeAsync(userId) + stream.Length > MaxStorage)
            {
                throw new InvalidOperationException("User has reached his storage capacity!");
            }

            var guid = Guid.NewGuid();
            var link = await this.dropboxService.UploadAsync(guid, stream);

            var file = new File
            {
                Id = guid,
                Filename = filename,
                Link = link,
                UserId = userId,
                Size = stream.Length,
            };

            await this.fileRepository.AddAsync(file);
            await this.fileRepository.SaveChangesAsync();

            return file.Link;
        }

        public Task<int> GetCountAsync()
            => this.fileRepository.AllAsNoTracking()
                .CountAsync();

        public async Task<ICollection<T>> GetAllForUserAsync<T>(string userId)
            where T : IMapFrom<File>
            => await this.fileRepository.AllAsNoTracking()
                .Where(x => x.UserId == userId)
                .To<T>()
                .ToListAsync();

        public Task<bool> IsFilenameAvailableAsync(string filename, string userId)
            => this.fileRepository.AllAsNoTracking()
                .AllAsync(x => x.Filename != filename || x.UserId != userId);

        public async Task RenameAsync(Guid id, string filename)
        {
            var file = await this.fileRepository.All()
                .FirstOrDefaultAsync(x => x.Id == id);
            file.Filename = filename;

            await this.fileRepository.SaveChangesAsync();
        }

        public Task<string> GetUserIdAsync(Guid id)
            => this.fileRepository.AllAsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => x.UserId)
                .FirstOrDefaultAsync();

        public async Task DeleteAsync(Guid id)
        {
            var file = await this.fileRepository.All()
                .FirstOrDefaultAsync(x => x.Id == id);

            await this.dropboxService.DeleteAsync(file.Id);

            this.fileRepository.HardDelete(file);
            await this.fileRepository.SaveChangesAsync();
        }

        public Task<long> GetUsedSizeAsync(string userId)
            => this.fileRepository.AllAsNoTracking()
                .Where(x => x.UserId == userId)
                .SumAsync(x => x.Size);
    }
}

namespace FactsGreet.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using FactsGreet.Services.Mapping;

    using File = FactsGreet.Data.Models.File;

    public interface IFilesService
    {
        public Task<string> UploadAsync(Stream stream, string filename, string userId);

        public Task<int> GetCountAsync();

        public Task<ICollection<T>> GetAllForUserAsync<T>(string userId)
            where T : IMapFrom<File>;

        public Task<bool> IsFilenameAvailableAsync(string filename, string userId);

        public Task RenameAsync(Guid id, string filename);

        public Task<string> GetUserIdAsync(Guid id);

        public Task DeleteAsync(Guid id);

        public Task<long> GetUsedSizeAsync(string userId);
    }
}

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
        Task<string> UploadAsync(Stream stream, string filename, string userId);

        Task<int> GetCountAsync();

        Task<ICollection<T>> GetAllForUserAsync<T>(string userId)
            where T : IMapFrom<File>;

        Task<bool> IsFilenameAvailableAsync(string filename, string userId);

        Task RenameAsync(Guid id, string filename);

        Task<string> GetUserIdAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<long> GetUsedSizeAsync(string userId);
    }
}

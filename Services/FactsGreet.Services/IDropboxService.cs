namespace FactsGreet.Services
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public interface IDropboxService
    {
        Task<string> UploadAsync(Guid id, Stream stream);

        Task DeleteAsync(Guid id);
    }
}

namespace FactsGreet.Services
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Dropbox.Api;
    using Dropbox.Api.Files;
    using Dropbox.Api.Sharing;

    public class DropboxService : IDropboxService
    {
        private readonly DropboxClient dropboxClient;

        public DropboxService(DropboxClient dropboxClient)
        {
            this.dropboxClient = dropboxClient;
        }

        public async Task<string> UploadAsync(Guid id, Stream stream)
        {
            var filename0 = "/" + id;
            await this.dropboxClient.Files.UploadAsync(filename0, WriteMode.Overwrite.Instance, body: stream);

            var sharedLinkMetadata = await this.dropboxClient.Sharing
                .CreateSharedLinkWithSettingsAsync(
                    filename0,
                    new SharedLinkSettings(RequestedVisibility.Public.Instance));

            return sharedLinkMetadata.Url.Replace("dl=0", "dl=1");
        }

        public Task DeleteAsync(Guid id)
        {
            return this.dropboxClient.Files.DeleteV2Async("/" + id);
        }
    }
}

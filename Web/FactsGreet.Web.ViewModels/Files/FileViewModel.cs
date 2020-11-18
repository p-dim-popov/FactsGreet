namespace FactsGreet.Web.ViewModels.Files
{
    using System;

    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public class FileViewModel : IMapFrom<File>
    {
        public Guid Id { get; set; }

        public string Filename { get; set; }

        public string Link { get; set; }

        public long Size { get; set; }
    }
}

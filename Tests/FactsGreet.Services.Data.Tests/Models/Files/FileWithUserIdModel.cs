namespace FactsGreet.Services.Data.Tests.Models.Files
{
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public class FileWithUserIdModel : IMapFrom<File>
    {
        public string UserId { get; set; }
    }
}

namespace FactsGreet.Services.Data.Tests.Models.ApplicationUsers
{
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public class ApplicationUserWithId : IMapFrom<ApplicationUser>
    {
        public string Id { get; set; }
    }
}

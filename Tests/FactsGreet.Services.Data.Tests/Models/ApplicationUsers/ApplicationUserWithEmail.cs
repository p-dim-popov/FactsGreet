namespace FactsGreet.Services.Data.Tests.Models.ApplicationUsers
{
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public class ApplicationUserWithEmail : IMapFrom<ApplicationUser>
    {
        public string NormalizedEmail { get; set; }
    }
}

namespace FactsGreet.Services.Data.TransferObjects.ApplicationUsers
{
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public class ApplicationUserWithId : IMapFrom<ApplicationUser>
    {
        public string Id { get; set; }
    }
}

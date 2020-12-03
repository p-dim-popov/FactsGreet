namespace FactsGreet.Web.ViewModels.Administration.Dashboard
{
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public class AdminRequestViewModel : IMapFrom<AdminRequest>
    {
        public string RequestSenderEmail { get; set; }

        public string MotivationalLetter { get; set; }
    }
}

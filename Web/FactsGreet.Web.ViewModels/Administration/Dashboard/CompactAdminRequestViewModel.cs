namespace FactsGreet.Web.ViewModels.Administration.Dashboard
{
    using System;

    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public class CompactAdminRequestViewModel : IMapFrom<AdminRequest>
    {
        public string RequestSenderEmail { get; set; }

        public Guid Id { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}

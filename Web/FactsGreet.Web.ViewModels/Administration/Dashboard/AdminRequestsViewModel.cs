namespace FactsGreet.Web.ViewModels.Administration.Dashboard
{
    using System.Collections.Generic;

    using FactsGreet.Web.ViewModels.Shared;

    public class AdminRequestsViewModel
    {
        public CompactPaginationViewModel CompactPaginationViewModel { get; set; }

        public ICollection<CompactAdminRequestViewModel> Requests { get; set; }
    }
}

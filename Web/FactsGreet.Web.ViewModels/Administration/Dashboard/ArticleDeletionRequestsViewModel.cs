﻿namespace FactsGreet.Web.ViewModels.Administration.Dashboard
{
    using System.Collections.Generic;

    using FactsGreet.Web.ViewModels.Shared;

    public class ArticleDeletionRequestsViewModel
    {
        public CompactPaginationViewModel CompactPaginationViewModel { get; set; }

        public ICollection<CompactArticleDeletionRequestViewModel> Requests { get; set; }
    }
}

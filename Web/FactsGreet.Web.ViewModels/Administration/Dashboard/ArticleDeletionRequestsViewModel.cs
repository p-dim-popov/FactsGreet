namespace FactsGreet.Web.ViewModels.Administration.Dashboard
{
    using System;
    using System.Collections.Generic;

    public class ArticleDeletionRequestsViewModel
    {
        private int page = 1;

        public int Page
        {
            get => this.page;
            set => this.page = Math.Max(1, value);
        }

        public ICollection<CompactArticleDeletionRequestViewModel> Requests { get; set; }
    }
}
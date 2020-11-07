namespace FactsGreet.Web.ViewModels.Shared
{
    using System;

    public class PaginationViewModel
    {
        private int currentPage;

        public int CurrentPage
        {
            get => this.currentPage;
            set => this.currentPage = Math.Max(1, value);
        }

        public int PagesCount { get; set; }

        public int ArticlesCount { get; set; }

        public int PreviousPage => this.CurrentPage switch {1 => 1, _ => this.CurrentPage - 1};

        public int NextPage => this.CurrentPage == this.PagesCount ? this.PagesCount : this.CurrentPage + 1;

        public string Slug { get; set; }

        public string ControllerName { get; set; }

        public string ActionName { get; set; }
    }
}
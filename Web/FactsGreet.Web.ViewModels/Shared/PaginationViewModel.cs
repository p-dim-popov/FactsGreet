namespace FactsGreet.Web.ViewModels.Shared
{
    using System;

    public class PaginationViewModel : CompactPaginationViewModel
    {
        public PaginationViewModel(
            int currentPage,
            int itemsPerPage,
            int itemsCount,
            Type controller = null,
            string action = null,
            object allRouteDataObject = null)
            : base(currentPage, controller, action, allRouteDataObject)
        {
            this.ItemsPerPage = itemsPerPage;
            this.ItemsCount = itemsCount;
        }

        public int PagesCount
            => (int)Math.Ceiling(1.0 * this.ItemsCount / this.ItemsPerPage);

        public int PreviousPage 
            => this.CurrentPage == 1 ? 1 : this.CurrentPage - 1;

        public int NextPage 
            => this.CurrentPage == this.PagesCount ? this.PagesCount : this.CurrentPage + 1;

        private int ItemsPerPage { get; }

        private int ItemsCount { get; }
    }
}

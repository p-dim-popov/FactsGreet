namespace FactsGreet.Web.ViewModels.Shared
{
    public class PaginationViewModel : CompactPaginationViewModel
    {
        public int PagesCount { get; set; }

        public int ItemsCount { get; set; }

        public int PreviousPage => this.CurrentPage switch { 1 => 1, _ => this.CurrentPage - 1 };

        public int NextPage => this.CurrentPage == this.PagesCount ? this.PagesCount : this.CurrentPage + 1;
    }
}

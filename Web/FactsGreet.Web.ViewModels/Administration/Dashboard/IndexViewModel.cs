namespace FactsGreet.Web.ViewModels.Administration.Dashboard
{
    public class IndexViewModel
    {
        public int JobsCount => this.ArticleDeletionRequestsCount + this.AdminRequestsCount;

        public int ArticleDeletionRequestsCount { get; set; }

        public int AdminRequestsCount { get; set; }
    }
}

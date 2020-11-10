namespace FactsGreet.Web.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using FactsGreet.Common;
    using FactsGreet.Services.Data;
    using FactsGreet.Web.ViewModels.Home;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        private const int EditsPerPage = 2;

        private readonly EditsService editsService;
        private readonly ArticlesService articlesService;

        public HomeController(EditsService editsService, ArticlesService articlesService)
        {
            this.editsService = editsService;
            this.articlesService = articlesService;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        public IActionResult TermsAndConditions()
        {
            return this.View();
        }

        [Authorize]
        public IActionResult Feed()
        {
            return this.View();
        }

        [Authorize]
        public async Task<IActionResult> GetFeedActivities(int page)
        {
            var activities = await this.GetFeedActivitiesPaginated(page);

            return this.PartialView("Partials/_FeedPartial", activities);
        }

        private async Task<ICollection<FeedViewModel>> GetFeedActivitiesPaginated(int page)
        {
            var pagination = Paginator.GetPagination(page, EditsPerPage);

            var activities =
                await this.editsService
                    .GetPaginatedOrderedByDateDescendingAsync<FeedViewModel>(
                        pagination.Skip, pagination.Take);
            return activities;
        }
    }
}
using System.Collections.Generic;
using FactsGreet.Web.ViewModels.Home;

namespace FactsGreet.Web.Controllers
{
    using System.Diagnostics;
    using System.Net.Mime;
    using System.Text;
    using System.Threading.Tasks;
    using FactsGreet.Common;
    using FactsGreet.Services.Data;
    using FactsGreet.Web.ViewModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Utf8Json;
    using Utf8Json.Resolvers;

    public class HomeController : BaseController
    {
        private const int EditsPerPage = 2;

        private readonly EditsService editsService;
        private readonly ArticlesService articlesService;

        public HomeController(EditsService editsService, ArticlesService articlesService)
        {
            this.editsService = editsService;
            this.articlesService = articlesService;

            JsonSerializer.SetDefaultResolver(StandardResolver.CamelCase); // TODO: move somewhere else
        }

        public IActionResult Index()
        {
            return this.View();
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(
                new ErrorViewModel {RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier});
        }

        [Authorize]
        public async Task<IActionResult> Feed()
        {
            return this.View(await this.GetFeedActivitiesPaginated(1));
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
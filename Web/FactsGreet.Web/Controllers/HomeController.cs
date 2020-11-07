namespace FactsGreet.Web.Controllers
{
    using System.Diagnostics;
    using System.Net.Mime;
    using System.Text;
    using System.Threading.Tasks;
    using FactsGreet.Common;
    using FactsGreet.Services.Data;
    using FactsGreet.Web.ViewModels;
    using FactsGreet.Web.ViewModels.Edits;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Utf8Json;
    using Utf8Json.Resolvers;

    public class HomeController : BaseController
    {
        private const int EditsPerPage = 2;

        private readonly EditsService editsService;

        public HomeController(EditsService editsService)
        {
            this.editsService = editsService;

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
        public IActionResult Feed()
        {
            return this.View();
        }

        [Authorize]
        public async Task<IActionResult> GetFeedActivities(int page)
        {
            var pagination = Paginator.GetPagination(page, EditsPerPage);
            var skip = (page - 1) * EditsPerPage;

            var activities =
                await this.editsService
                    .GetPaginatedOrderedByDateDescendingAsync<EditViewModel>(
                        pagination.Skip, pagination.Take);

            return this.Content(
                JsonSerializer.ToJsonString(activities),
                MediaTypeNames.Application.Json,
                Encoding.UTF8);
        }
    }
}
namespace FactsGreet.Web.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using FactsGreet.Common;
    using FactsGreet.Services.Data;
    using FactsGreet.Web.ViewModels.Edits;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class EditsController : BaseController
    {
        private const int EditsPerPage = 3;

        private readonly EditsService editsService;

        public EditsController(EditsService editsService)
        {
            this.editsService = editsService;
        }

        [Route("[controller]/[action]/{title}", Name = "edits_create")]
        public IActionResult Create(string title)
        {
            return this.View();
        }

        [Route("[controller]/[action]/{title}", Name = "edits_history")]
        public IActionResult History(string title)
        {
            return this.View();
        }

        [Authorize]
        public async Task<IActionResult> GetEdits(int page = 1, string userId = null)
        {
            var edits = await this.GetEditsPaginated(page, userId);

            return this.PartialView("_ListCompactEditsPartial", edits);
        }

        private async Task<ICollection<CompactEditViewModel>> GetEditsPaginated(int page, string userId = null)
        {
            var pagination = Paginator.GetPagination(page, EditsPerPage);

            var activities =
                await this.editsService
                    .GetPaginatedOrderedByDateDescendingAsync<CompactEditViewModel>(
                        pagination.Skip, pagination.Take, userId);
            return activities;
        }
    }
}

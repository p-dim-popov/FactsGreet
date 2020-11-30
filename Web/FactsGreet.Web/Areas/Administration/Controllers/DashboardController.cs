namespace FactsGreet.Web.Areas.Administration.Controllers
{
    using System;
    using System.Threading.Tasks;

    using FactsGreet.Common;
    using FactsGreet.Services.Data;
    using FactsGreet.Services.Data.Implementations;
    using FactsGreet.Web.Infrastructure;
    using FactsGreet.Web.ViewModels.Administration.Dashboard;
    using Microsoft.AspNetCore.Mvc;

    public class DashboardController : AdministrationController
    {
        private const int DeletionRequestsPerPage = 3;

        private readonly NotificationsService notificationsService;
        private readonly ArticleDeletionRequestsService articleDeletionRequestsService;

        public DashboardController(
            NotificationsService notificationsService,
            ArticleDeletionRequestsService articleDeletionRequestsService)
        {
            this.notificationsService = notificationsService;
            this.articleDeletionRequestsService = articleDeletionRequestsService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new IndexViewModel
            {
                NotificationsCount = await this.notificationsService.GetCountAsync(),
                ArticleDeletionRequestNotificationsCount = await this.articleDeletionRequestsService.GetCountAsync(),
            };
            return this.View(viewModel);
        }

        public async Task<IActionResult> ArticleDeletionRequests(int page = 1)
        {
            var pagination = Paginator.GetPagination(page, DeletionRequestsPerPage);
            return this.View(new ArticleDeletionRequestsViewModel
            {
                Page = page,
                Requests = await this.articleDeletionRequestsService
                    .GetPaginatedOrderedByCreationDateAsync<CompactArticleDeletionRequestViewModel>(
                        pagination.Skip,
                        pagination.Take),
            });
        }

        public async Task<IActionResult> ArticleDeletionRequest(Guid id)
        {
            return this.View(await this.articleDeletionRequestsService
                .GetById<ArticleDeletionRequestViewModel>(id));
        }

        public IActionResult MostActiveUsers(int page)
        {
            return this.View();
        }

        public IActionResult View(Guid id)
        {
            return this.View();
        }
    }
}

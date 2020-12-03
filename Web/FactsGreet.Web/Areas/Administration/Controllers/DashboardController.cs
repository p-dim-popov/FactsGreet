namespace FactsGreet.Web.Areas.Administration.Controllers
{
    using System;
    using System.Threading.Tasks;

    using FactsGreet.Services.Data.Implementations;
    using FactsGreet.Web.Infrastructure;
    using FactsGreet.Web.ViewModels.Administration.Dashboard;
    using FactsGreet.Web.ViewModels.Shared;
    using Microsoft.AspNetCore.Mvc;

    public class DashboardController : AdministrationController
    {
        private const int DeletionRequestsPerPage = 3;

        private readonly ArticleDeletionRequestsService articleDeletionRequestsService;
        private readonly AdminRequestsService adminRequestsService;

        public DashboardController(
            ArticleDeletionRequestsService articleDeletionRequestsService, AdminRequestsService adminRequestsService)
        {
            this.articleDeletionRequestsService = articleDeletionRequestsService;
            this.adminRequestsService = adminRequestsService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new IndexViewModel
            {
                ArticleDeletionRequestsCount = await this.articleDeletionRequestsService.GetCountAsync(),
                AdminRequestsCount = await this.adminRequestsService.GetCountAsync(),
            };
            return this.View(viewModel);
        }

        public async Task<IActionResult> ArticleDeletionRequests(int page)
        {
            var pagination = Paginator.GetPagination(page, DeletionRequestsPerPage);
            return this.View(new ArticleDeletionRequestsViewModel
            {
                CompactPaginationViewModel = new CompactPaginationViewModel(page),
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

        public async Task<IActionResult> AdminRequests(int page)
        {
            var pagination = Paginator.GetPagination(page, DeletionRequestsPerPage);
            return this.View(new AdminRequestsViewModel
            {
                CompactPaginationViewModel = new CompactPaginationViewModel(page),
                Requests = await this.adminRequestsService
                    .GetPaginatedOrderedByCreationDateAsync<CompactAdminRequestViewModel>(
                        pagination.Skip,
                        pagination.Take),
            });
        }

        public async Task<IActionResult> AdminRequest(Guid id)
        {
            return this.View(await this.adminRequestsService
                .GetById<AdminRequestViewModel>(id));
        }
    }
}

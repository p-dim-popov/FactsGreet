namespace FactsGreet.Web.Areas.Administration.Controllers
{
    using System;
    using System.Threading.Tasks;

    using FactsGreet.Services.Data;
    using FactsGreet.Services.Data.Implementations;
    using FactsGreet.Web.Infrastructure;
    using FactsGreet.Web.ViewModels.Administration.Dashboard;
    using FactsGreet.Web.ViewModels.Shared;
    using Microsoft.AspNetCore.Mvc;

    public class DashboardController : AdministrationController
    {
        private const int ArticleDeletionRequestsPerPage = 3;
        private const int AdminRequestsPerPage = 3;

        private readonly IArticleDeletionRequestsService articleDeletionRequestsService;
        private readonly IAdminRequestsService adminRequestsService;
        private readonly IApplicationUsersService applicationUsersService;

        public DashboardController(
            IArticleDeletionRequestsService articleDeletionRequestsService,
            IAdminRequestsService adminRequestsService,
            IApplicationUsersService applicationUsersService)
        {
            this.articleDeletionRequestsService = articleDeletionRequestsService;
            this.adminRequestsService = adminRequestsService;
            this.applicationUsersService = applicationUsersService;
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
            var pagination = Paginator.GetPagination(page, ArticleDeletionRequestsPerPage);
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

        public async Task<IActionResult> MostActiveUsers()
        {
            return this.View(await this.applicationUsersService
                .Get10MostActiveUsersForTheLastWeekAsync<ActiveUserViewModel>());
        }

        public async Task<IActionResult> AdminRequests(int page)
        {
            var pagination = Paginator.GetPagination(page, AdminRequestsPerPage);
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

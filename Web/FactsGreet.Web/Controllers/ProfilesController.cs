namespace FactsGreet.Web.Controllers
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using FactsGreet.Common;
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Data.Implementations;
    using FactsGreet.Web.Areas.Administration.Controllers;
    using FactsGreet.Web.Infrastructure;
    using FactsGreet.Web.ViewModels.Profiles;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class ProfilesController : BaseController
    {
        private readonly ApplicationUsersService applicationUsersService;
        private readonly FollowsService followsService;
        private readonly AdminRequestsService adminRequestsService;
        private readonly UserManager<ApplicationUser> userManager;

        public ProfilesController(
            ApplicationUsersService applicationUsersService,
            FollowsService followsService,
            AdminRequestsService adminRequestsService,
            UserManager<ApplicationUser> userManager)
        {
            this.applicationUsersService = applicationUsersService;
            this.followsService = followsService;
            this.adminRequestsService = adminRequestsService;
            this.userManager = userManager;
        }

        [HttpGet("[controller]/View/{email}", Name = "profile_index")]
        public async Task<IActionResult> Index(string email)
        {
            var profile = await this.applicationUsersService.GetByEmailAsync<ProfileIndexViewModel>(email);

            return this.View("View", profile);
        }

        public async Task<IActionResult> RemoveBadge(string name)
        {
            await this.applicationUsersService
                .RemoveBadgeAsync(this.UserId, name);

            return this.RedirectToRoute(
                "profile_index",
                new { email = this.User.FindFirstValue(ClaimTypes.Email) });
        }

        public async Task<IActionResult> Follow(string userId)
        {
            await this.followsService.Follow(this.UserId, userId);
            return this.RedirectToRoute(
                "profile_index",
                new { email = await this.applicationUsersService.GetEmailAsync(userId) });
        }

        public async Task<IActionResult> Unfollow(string userId)
        {
            await this.followsService.Unfollow(this.UserId, userId);
            return this.RedirectToRoute(
                "profile_index",
                new { email = await this.applicationUsersService.GetEmailAsync(userId) });
        }

        public IActionResult WhoAmI()
        {
            return this.Json(this.UserId);
        }

        public async Task<IActionResult> Get10EmailsByEmailKeyword(string keyword)
        {
            return this.Json(await this.applicationUsersService.Get10EmailsByEmailKeywordAsync(keyword));
        }

        [Authorize(Roles = GlobalConstants.RegularRoleName)]
        public IActionResult CreateAdminRequest()
        {
            return this.View();
        }

        [HttpPost]
        [Authorize(Roles = GlobalConstants.RegularRoleName)]
        public async Task<IActionResult> CreateAdminRequest(CreateAdminRequestInputModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            await this.adminRequestsService.CreateAsync(this.UserId, model.MotivationalLetter);

            return this.RedirectToRoute(
                "profile_index",
                new { email = this.User.FindFirstValue(ClaimTypes.Email) });
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> AddToRole(string email, string role)
        {
            var user = await this.userManager.FindByEmailAsync(email);

            if (role is GlobalConstants.AdministratorRoleName)
            {
                await this.userManager.RemoveFromRoleAsync(user, GlobalConstants.RegularRoleName);
                await this.adminRequestsService.DeleteForUserIdAsync(user.Id);
                await this.userManager.AddToRoleAsync(user, role);
                return this.RedirectToRoute(
                    new
                    {
                        area = "Administration",
                        controller = "Dashboard",
                        action = "AdminRequests",
                    });
            }

            return this.RedirectToRoute(new { area = "Administration", controller = "Dashboard", action = "Index" });
        }
    }
}

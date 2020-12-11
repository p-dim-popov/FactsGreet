﻿namespace FactsGreet.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using FactsGreet.Common;
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Data;
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
        private readonly IApplicationUsersService applicationUsersService;
        private readonly FollowsService followsService;
        private readonly AdminRequestsService adminRequestsService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly BadgesService badgesService;

        public ProfilesController(
            IApplicationUsersService applicationUsersService,
            FollowsService followsService,
            AdminRequestsService adminRequestsService,
            UserManager<ApplicationUser> userManager,
            BadgesService badgesService)
        {
            this.applicationUsersService = applicationUsersService;
            this.followsService = followsService;
            this.adminRequestsService = adminRequestsService;
            this.userManager = userManager;
            this.badgesService = badgesService;
        }

        [HttpGet("[controller]/View/{email}", Name = "profile_index")]
        public async Task<IActionResult> Index(string email)
        {
            var profile = await this.applicationUsersService.GetByEmailAsync<ProfileIndexViewModel>(email);

            return this.View("View", profile);
        }

        public async Task<IActionResult> Follow(string userId)
        {
            await this.followsService.Follow(this.UserId, userId);
            return this.RedirectToRoute(
                Helpers.GetRouteNames(this.GetType(), nameof(this.Index)).FirstOrDefault(),
                new { email = await this.applicationUsersService.GetEmailAsync(userId) });
        }

        public async Task<IActionResult> Unfollow(string userId)
        {
            await this.followsService.Unfollow(this.UserId, userId);
            return this.RedirectToRoute(
                Helpers.GetRouteNames(this.GetType(), nameof(this.Index)).FirstOrDefault(),
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
                Helpers.GetRouteNames(this.GetType(), nameof(this.Index)).FirstOrDefault(),
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

        public async Task<IActionResult> RemoveBadge(string name)
        {
            await this.badgesService
                .RemoveBadgeFromUserAsync(this.UserId, name);

            return this.RedirectToRoute(new
            {
                action = "Index",
                email = this.User.FindFirstValue(ClaimTypes.Email),
            });
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        [HttpPost]
        public async Task<IActionResult> AddBadge(string email, string badge)
        {
            await this.badgesService
                .AddBadgeToUserAsync(await this.applicationUsersService.GetIdByEmailAsync(email), badge);

            return this.RedirectToRoute(new
            {
                area = "Administration",
                controller = "Dashboard",
                action = "MostActiveUsers",
            });
        }
    }
}

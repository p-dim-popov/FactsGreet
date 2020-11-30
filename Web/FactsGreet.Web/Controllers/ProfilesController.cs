namespace FactsGreet.Web.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using FactsGreet.Services.Data;
    using FactsGreet.Services.Data.Implementations;
    using FactsGreet.Web.ViewModels.Profiles;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class ProfilesController : BaseController
    {
        private readonly ApplicationUsersService applicationUsersService;
        private readonly FollowsService followsService;

        public ProfilesController(
            ApplicationUsersService applicationUsersService,
            FollowsService followsService)
        {
            this.applicationUsersService = applicationUsersService;
            this.followsService = followsService;
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

        // TODO: View all starred articles
    }
}

namespace FactsGreet.Web.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using FactsGreet.Services.Data;
    using FactsGreet.Web.ViewModels.Profiles;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class ProfilesController : BaseController
    {
        private readonly ApplicationUsersService applicationUsersService;

        public ProfilesController(ApplicationUsersService applicationUsersService)
        {
            this.applicationUsersService = applicationUsersService;
        }

        [Authorize]
        [HttpGet("[controller]/View/{email}")]
        public async Task<IActionResult> Index(string email)
        {
            var profile = await this.applicationUsersService.GetByEmailAsync<ProfileIndexViewModel>(email);

            return this.View("View", profile);
        }

        [Authorize]
        public async Task<IActionResult> RemoveBadge(string name)
        {
            await this.applicationUsersService
                .RemoveBadgeAsync(this.User.FindFirstValue(ClaimTypes.NameIdentifier), name);

            return this.RedirectToRoute($"View/{this.User.FindFirstValue(ClaimTypes.Email)}");
        }
    }
}
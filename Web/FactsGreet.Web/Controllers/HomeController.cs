namespace FactsGreet.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            return this.View();
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        public IActionResult TermsAndConditions()
        {
            return this.View();
        }

        [Authorize]
        public IActionResult Feed()
        {
            return this.View();
        }
    }
}

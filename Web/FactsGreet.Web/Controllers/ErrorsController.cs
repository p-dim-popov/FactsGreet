namespace FactsGreet.Web.Controllers
{
    using System.Diagnostics;

    using FactsGreet.Web.ViewModels;
    using FactsGreet.Web.ViewModels.Home;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.WebUtilities;

    public class ErrorsController : BaseController
    {
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Route("/[action]")]
        public IActionResult Error()
        {
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }

        public IActionResult StatusCodePage(int id)
        {
            return this.View(new StatusCodePageViewModel
            {
                StatusCode = id,
                StatusText = ReasonPhrases.GetReasonPhrase(id),
            });
        }
    }
}

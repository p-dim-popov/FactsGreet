namespace FactsGreet.Web.Controllers
{
    using System.Security.Claims;

    using Microsoft.AspNetCore.Mvc;

    public class BaseController : Controller
    {
        protected string UserId
            => this.User.FindFirstValue(ClaimTypes.NameIdentifier);

        public override string ToString()
            => this.GetType().Name.Replace("Controller", string.Empty);
    }
}

namespace FactsGreet.Web.Controllers
{
    using System;

    using FactsGreet.Services.Data;
    using Microsoft.AspNetCore.Mvc;

    public class EditsController : BaseController
    {
        public EditsController(EditsService editsService)
        { }

        [HttpGet("[controller]/Create/{title}")]
        public IActionResult Create(string title)
        {
            return this.View();
        }
    }
}
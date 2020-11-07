using System.Threading.Tasks;
using FactsGreet.Web.ViewModels.Edits;

namespace FactsGreet.Web.Controllers
{
    using System;

    using FactsGreet.Services.Data;
    using Microsoft.AspNetCore.Mvc;

    public class EditsController : BaseController
    {
        private const int ArticlesPerPage = 1;

        private readonly EditsService editsService;

        public EditsController(EditsService editsService)
        {
            this.editsService = editsService;
        }
    }
}
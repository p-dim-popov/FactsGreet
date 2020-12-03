namespace FactsGreet.Web.ViewModels.Shared
{
    using System;
    using System.Collections.Generic;

    using FactsGreet.Common;

    using Extensions = FactsGreet.Web.Infrastructure.Extensions;

    public class CompactPaginationViewModel
    {
        private readonly int currentPage = 1;
        private readonly object allRouteDataObject;

        public CompactPaginationViewModel(
            int currentPage,
            Type controller = null,
            string action = null,
            object allRouteDataObject = null)
        {
            this.CurrentPage = currentPage;
            this.Controller = controller;
            this.Action = action;
            this.allRouteDataObject = allRouteDataObject ?? new { };
        }

        public int CurrentPage
        {
            get => this.currentPage;
            private init => this.currentPage = Math.Max(1, value);
        }

        public string Route
            => Extensions.FirstOrDefaultNotNullRouteName(this.Controller, this.Action);

        private Type Controller { get; }

        private string Action { get; }

        public IDictionary<string, string> GetAllRouteDataForPage(int page)
            => this.allRouteDataObject
                .ToDictionary()
                .AppendElement("page", page.ToString());
    }
}

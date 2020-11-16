namespace FactsGreet.Web.ViewModels.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CompactPaginationViewModel
    {
        private int currentPage = 1;

        public int CurrentPage
        {
            get => this.currentPage;
            set => this.currentPage = Math.Max(1, value);
        }

        public string Path { get; set; }

        public ICollection<(string Key, object Value)> Query { get; set; }
            = new HashSet<(string Key, object Value)>();

        public string QueryString
            => string.Join('&', this.Query
                .Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value.ToString())}"));

        public string Url => $"{this.Path}?{this.QueryString}";
    }
}

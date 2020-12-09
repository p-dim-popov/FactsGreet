namespace FactsGreet.Web.ViewModels.Administration.Dashboard
{
    using System;

    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public class MostActiveUsersEditViewModel : IMapFrom<Edit>
    {
        public Guid Id { get; set; }

        public bool IsCreation { get; set; }

        public string ArticleTitle { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}

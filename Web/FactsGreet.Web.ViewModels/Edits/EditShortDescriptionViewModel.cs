namespace FactsGreet.Web.ViewModels.Edits
{
    using System;

    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public class EditShortDescriptionViewModel : IMapFrom<Edit>
    {
        public Guid Id { get; set; }

        public string EditorUserName { get; set; }

        public DateTime CreatedOn { get; set; }

        public int ModificationsCount { get; set; }

        public string Comment { get; set; }
    }
}

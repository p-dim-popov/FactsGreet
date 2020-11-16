namespace FactsGreet.Web.ViewModels.Edits
{
    using System.Collections.Generic;

    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public class EditViewModel : IMapFrom<Edit>
    {
        public string EditorUserName { get; set; }

        public string ArticleTitle { get; set; }
        
        public ICollection<Diff> Modifications { get; set; }
    }
}

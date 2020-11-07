namespace FactsGreet.Web.ViewModels.Edits
{
    using AutoMapper;
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;
    using FactsGreet.Web.ViewModels.Articles;

    public class EditViewModel : IMapFrom<Edit>, IHaveCustomMappings
    {
        public CompactArticleViewModel Article { get; set; }

        public string EditorUserName { get; set; }

        public bool IsCreation { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Edit, EditViewModel>()
                .ForMember(
                    x => x.EditorUserName,
                    opt => opt
                        .MapFrom(x => x.Editor.UserName));
        }
    }
}
namespace FactsGreet.Web.ViewModels.Edits
{
    using System;
    using AutoMapper;
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;
    using FactsGreet.Web.ViewModels.Articles;

    public class EditWithCompactArticleViewModel : IMapFrom<Edit>
    {
        public Guid Id { get; set; }

        public CompactArticleViewModel Article { get; set; }

        public string EditorUserName { get; set; }

        public bool IsCreation { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Edit, EditWithCompactArticleViewModel>()
                .ForMember(
                    x => x.EditorUserName,
                    opt => opt
                        .MapFrom(x => x.Editor.UserName));
        }
    }
}

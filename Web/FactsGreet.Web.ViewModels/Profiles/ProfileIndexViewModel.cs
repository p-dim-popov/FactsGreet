using FactsGreet.Web.ViewModels.Edits;

namespace FactsGreet.Web.ViewModels.Profiles
{
    using System.Collections.Generic;

    using AutoMapper;
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;
    using FactsGreet.Web.ViewModels.Home;

    public class ProfileIndexViewModel : IMapFrom<ApplicationUser>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public virtual ICollection<Badge> Badges { get; set; }
            = new HashSet<Badge>();

        public virtual ICollection<EditWithCompactArticleViewModel> Activities { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ApplicationUser, ProfileIndexViewModel>()
                .ForMember(
                    x => x.Activities,
                    opt => opt
                        .MapFrom(x => x.Edits));
        }
    }
}
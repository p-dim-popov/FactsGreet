namespace FactsGreet.Web.ViewModels.Administration.Dashboard
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public class ActiveUserViewModel : IMapFrom<ApplicationUser>, IHaveCustomMappings
    {
        public string Email { get; set; }

        public ICollection<MostActiveUsersEditViewModel> Edits { get; set; }

        public ICollection<Badge> Badges { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ApplicationUser, ActiveUserViewModel>()
                .ForMember(
                    x => x.Edits,
                    opt => opt
                        .MapFrom(x => x.Edits.Where(y => y.CreatedOn > DateTime.UtcNow.AddDays(-7))));
        }
    }
}

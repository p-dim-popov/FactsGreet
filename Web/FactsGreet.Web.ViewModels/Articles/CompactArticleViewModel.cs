namespace FactsGreet.Web.ViewModels.Articles
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public class CompactArticleViewModel : IMapFrom<Article>, IHaveCustomMappings
    {
        public string Title { get; set; }

        public string ShortContent { get; set; }

        public string Description { get; set; }

        public ICollection<string> Categories { get; set; }

        public int StarsCount { get; set; }

        public string ThumbnailLink { get; set; }

        public virtual void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Article, CompactArticleViewModel>()
                .ForMember(
                    m => m.Categories,
                    opt => opt
                        .MapFrom(x => x.Categories
                            .Select(y => y.Category.Name)))
                .ForMember(
                    m => m.ShortContent,
                    opt =>
                    {
                        opt.PreCondition(src => src.Description == null);
                        opt.MapFrom(x => x.Content
                                .Substring(0, 300)
                                .Substring(0, x.Content
                                    .Substring(0, 300).LastIndexOf(' ')) +
                            "...");
                    });
        }
    }
}
namespace FactsGreet.Web.ViewModels.Conversations
{
    using System.Linq;

    using AutoMapper;
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;

    public class ConversationInfoViewModel : ConversationViewModel, IMapFrom<Conversation>, IHaveCustomMappings
    {
        public MessageViewModel Message { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Conversation, ConversationInfoViewModel>()
                .ForMember(
                    x => x.Message,
                    opt => opt
                        .MapFrom(x => x.Messages
                            .OrderByDescending(y => y.CreatedOn)
                            .Select(x => new MessageViewModel
                            {
                                Id = x.Id,
                                Content = x.Content,
                                ConversationId = x.ConversationId,
                                CreatedOn = x.CreatedOn,
                                SenderId = x.SenderId,
                                SenderUserName = x.Sender.UserName,
                            })
                            .FirstOrDefault()));
        }
    }
}

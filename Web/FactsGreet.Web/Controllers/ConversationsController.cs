namespace FactsGreet.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using FactsGreet.Services.Data;
    using FactsGreet.Services.Data.Implementations;
    using FactsGreet.Web.Hubs;
    using FactsGreet.Web.ViewModels.Conversations;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;

    [Authorize]
    public class ConversationsController : BaseController
    {
        private const int MessagesPerPage = 3;
        private const int ConversationsPerPage = 3;

        private readonly ConversationsService conversationsService;
        private readonly IMessagesService messagesService;
        private readonly IApplicationUsersService applicationUsersService;
        private readonly FollowsService followsService;
        private readonly IHubContext<ChatHub> chatHubContext;

        public ConversationsController(
            ConversationsService conversationsService,
            IMessagesService messagesService,
            IApplicationUsersService applicationUsersService,
            FollowsService followsService,
            IHubContext<ChatHub> chatHubContext)
        {
            this.conversationsService = conversationsService;
            this.messagesService = messagesService;
            this.applicationUsersService = applicationUsersService;
            this.followsService = followsService;
            this.chatHubContext = chatHubContext;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        public async Task<IActionResult> Messages(string email, Guid? id)
        {
            ConversationViewModel convo;

            if (id.HasValue)
            {
                convo = await this.conversationsService.GetAsync<ConversationViewModel>(id.Value);
            }
            else
            {
                var targetUserId = await this.applicationUsersService.GetIdByEmailAsync(email);
                if (targetUserId is null)
                {
                    return this.NotFound();
                }

                // If conversation already exists return it
                convo = await this.conversationsService.GetAsync<ConversationViewModel>(
                    this.UserId,
                    targetUserId);

                // I think it's better to keep logic flow and keep only one correct return at the end
                // ReSharper disable once InvertIf
                if (convo is null)
                {
                    // Check if user requesting the conversation creation in fact is followed by the target user (reduce unknown spam)
                    if (!await this.followsService.IsUserFollowingUserAsync(targetUserId, this.UserId))
                    {
                        return this.Forbid();
                    }

                    // If the user is followed by the target, allow conversation creation
                    convo = await this.conversationsService
                        .CreateAsync<ConversationViewModel>(
                            this.UserId,
                            targetUserId);
                }
            }

            return this.View(new MessagesViewModel
            {
                ConversationId = convo.Id,
                ConversationTitle = convo.Title,
            });
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromForm] MessageInputModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Problem();
            }

            if (!await this.conversationsService.IsUserParticipantAsync(model.ConversationId, this.UserId))
            {
                return this.Forbid();
            }

            var messageId =
                await this.conversationsService.SendMessageAsync(this.UserId, model.ConversationId, model.Content);
            await this.chatHubContext.Clients
                .Users(await this.conversationsService.GetParticipantsAsync(model.ConversationId))
                .SendAsync(
                    "ReceiveMessage",
                    new ReceiveMessageViewModel
                    {
                        Id = messageId,
                        ConversationId = model.ConversationId,
                    });

            return this.Ok();
        }

        public async Task<IActionResult> GetMessage(Guid id)
        {
            if (!await this.messagesService.CanUserGetMessageAsync(id, this.UserId))
            {
                return this.Forbid();
            }

            return this.Json(await this.messagesService.GetByIdAsync<MessageViewModel>(id));
        }

        public async Task<IActionResult> GetMessagesPage(Guid conversationId, Guid? referenceMessageId)
        {
            if (!await this.conversationsService.IsUserParticipantAsync(conversationId, this.UserId))
            {
                return this.Forbid();
            }

            return this.Json(
                await this.messagesService.GetFewOlderThanAsync<MessageViewModel>(
                    referenceMessageId,
                    conversationId,
                    MessagesPerPage));
        }

        public async Task<IActionResult> GetConversationsPage(Guid? referenceConversationId)
        {
            return this.Json(
                await this.conversationsService
                    .GetFewOlderThanAsync<ConversationInfoViewModel>(
                        referenceConversationId,
                        this.UserId,
                        ConversationsPerPage));
        }

        public async Task<IActionResult> GetConversationTitle(Guid id)
        {
            if (!await this.conversationsService.IsUserParticipantAsync(id, this.UserId))
            {
                return this.Forbid();
            }

            return this.Json(await this.conversationsService.GetTitleAsync(id));
        }

        public async Task<IActionResult> GetConversationsIdsByEmails(string users)
        {
            return this.Json(await this.conversationsService
                .GetByUsersEmailsAsync<ConversationViewModel>(
                    users
                        .Split(",", StringSplitOptions.RemoveEmptyEntries)
                        .Append(this.User.FindFirstValue(ClaimTypes.Email))));
        }
    }
}

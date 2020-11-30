namespace FactsGreet.Services.Data.Implementations
{
    using System.Threading.Tasks;
    using FactsGreet.Data.Common.Repositories;
    using FactsGreet.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class NotificationsService
    {
        private readonly IRepository<Notification> notificationRepository;

        public NotificationsService(
            IRepository<Notification> notificationRepository)
        {
            this.notificationRepository = notificationRepository;
        }

        public Task<int> GetCountAsync()
            => this.notificationRepository.AllAsNoTracking().CountAsync();
    }
}
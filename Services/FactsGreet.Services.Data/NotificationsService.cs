using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FactsGreet.Services.Data
{
    using FactsGreet.Data.Common.Repositories;
    using FactsGreet.Data.Models;

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
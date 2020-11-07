namespace FactsGreet.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using FactsGreet.Data.Common.Repositories;
    using FactsGreet.Data.Models;
    using FactsGreet.Services.Mapping;
    using Microsoft.EntityFrameworkCore;

    public class EditsService
    {
        private readonly IDeletableEntityRepository<Edit> editsRepository;

        public EditsService(IDeletableEntityRepository<Edit> editsRepository)
        {
            this.editsRepository = editsRepository;
        }

        public async Task<ICollection<T>> GetPaginatedOrderedByDateDescendingAsync<T>(int skip, int take)
        {
            return await this.editsRepository.All()
                .OrderByDescending(x => x.CreatedOn)
                .To<T>()
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }
    }
}
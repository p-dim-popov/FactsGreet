namespace FactsGreet.Data.Seeding
{
    using System;
    using System.Threading.Tasks;

    public interface ISeeder
    {
        protected static readonly Random Random = new Random(DateTime.Now.Millisecond);

        Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider);
    }
}

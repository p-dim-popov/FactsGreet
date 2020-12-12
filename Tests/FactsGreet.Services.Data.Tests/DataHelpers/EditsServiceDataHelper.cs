namespace FactsGreet.Services.Data.Tests.DataHelpers
{
    using System.Collections.Generic;
    using System.Linq;
    using FactsGreet.Data.Models;

    public static class EditsServiceDataHelper
    {
        public static ICollection<Edit> GetEdits(this EditsServiceTests tests)
            => Enumerable.Range(1, 10)
                .Select(x => new Edit{ })
                .ToList();
    }
}

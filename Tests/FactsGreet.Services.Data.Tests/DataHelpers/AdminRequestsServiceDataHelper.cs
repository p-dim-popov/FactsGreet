namespace FactsGreet.Services.Data.Tests.DataHelpers
{
    using System.Collections.Generic;
    using System.Linq;
    using FactsGreet.Data.Models;

    public static class AdminRequestsServiceDataHelper
    {
        public static ICollection<AdminRequest> GetRequests(this AdminRequestsServiceTests tests)
            => Enumerable.Range(1, 10)
                .Select(x => new AdminRequest
                {
                    
                })
                .ToList();
    }
}

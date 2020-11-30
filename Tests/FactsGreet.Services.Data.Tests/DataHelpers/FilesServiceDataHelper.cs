namespace FactsGreet.Services.Data.Tests.DataHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FactsGreet.Data.Models;

    public static class FilesServiceDataHelper
    {
        public static IEnumerable<File> GetFiles(this FilesServiceTests tests, int count = 5)
            => Enumerable.Range(1, count)
                .Select(x => new File
                    { Id = Guid.NewGuid(), Filename = $"filename_{x}", UserId = x.ToString(), Size = x });
    }
}

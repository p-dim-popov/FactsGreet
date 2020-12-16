namespace FactsGreet.Services.Data.Tests.DataHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FactsGreet.Data.Models;

    public static class BadgesServiceDataHelper
    {
        public static ICollection<Badge> GetBadges(this BadgesServiceTests tests)
            => Enumerable.Range(1, 10)
                .Select(x => new Badge
                {
                    Id = Guid.NewGuid(),
                    Name = x.ToString(),
                })
                .ToList();

        public static (ICollection<ApplicationUser> Users, ICollection<Badge> Badges) GetUsersWithBadges(this BadgesServiceTests tests)
        {
            var rng = new Random(DateTime.UtcNow.Millisecond);
            var badges = GetBadges(null).OrderBy(_ => rng.Next()).ToList();
            var users = ApplicationUsersServiceDataHelper.GetUsers(null)
                .Select(x =>
                {
                    for (int i = 0; i < rng.Next(1, badges.Count); i++)
                    {
                        x.Badges.Add(badges[rng.Next(0, badges.Count)]);
                    }

                    x.Badges = x.Badges.Distinct().ToList();
                    return x;
                })
                .OrderBy(_ => rng.Next())
                .ToList();

            return (users, badges);
        }
    }
}

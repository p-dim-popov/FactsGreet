namespace FactsGreet.Services.Data.Tests.DataHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FactsGreet.Data.Models;

    public static class FollowsServiceDataHelper
    {
        public static ICollection<ApplicationUser> GetFollowedUsers(this FollowsServiceTests tests)
        {
            var rng = new Random();
            var users = ApplicationUsersServiceDataHelper.GetUsers(null)
                .OrderBy(_ => rng.Next())
                .ToList();

            return users
                .Select(x =>
                {
                    var usersWithoutMe = users.Where(y => y.Id != x.Id).ToList();
                    x.Followers.Add(new Follow
                    {
                        FollowerId = usersWithoutMe[rng.Next(0, usersWithoutMe.Count)].Id,
                        FollowedId = x.Id,
                    });

                    x.Followings.Add(new Follow
                    {
                        FollowerId = x.Id,
                        FollowedId = usersWithoutMe[rng.Next(0, usersWithoutMe.Count)].Id,
                    });
                    return x;
                })
                .ToList();
        }
    }
}

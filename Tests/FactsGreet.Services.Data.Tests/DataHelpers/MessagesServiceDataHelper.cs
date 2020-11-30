namespace FactsGreet.Services.Data.Tests.DataHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using FactsGreet.Common;
    using FactsGreet.Data.Models;
    using NLipsum.Core;

    public static class MessagesServiceDataHelper
    {
        public static IEnumerable<Message> GetMessages(this MessagesServiceTests tests, int count = 5)
            => Enumerable.Range(1, count)
                .Select(x => new Message
                {
                    Id = Guid.NewGuid(),
                    Content = new LipsumGenerator().GenerateSentences(2).Join(string.Empty),
                    Conversation = new Conversation
                    {
                        Users = Enumerable.Range(1, count).Select(_ => new ApplicationUser()).ToList(),
                    },
                    CreatedOn = DateTime.UtcNow,
                });

        public static IEnumerable<Message> GetMessagesFromConversation1(this MessagesServiceTests tests, int count = 5)
        {
            var data = new MessagesServiceData();
            return Enumerable.Range(1, count)
                .Select(x => new Message
                {
                    Id = Guid.NewGuid(),
                    Content = new LipsumGenerator().GenerateSentences(2).Join(string.Empty),
                    Conversation = data.ConversationFirstUsers,
                    CreatedOn = DateTime.UtcNow,
                });
        }

        public static IEnumerable<Message> GetMessagesFromConversation2(this MessagesServiceTests tests, int count = 5)
        {
            var data = new MessagesServiceData();
            return Enumerable.Range(1, count)
                .Select(x => new Message
                {
                    Id = Guid.NewGuid(),
                    Content = new LipsumGenerator().GenerateSentences(2).Join(string.Empty),
                    Conversation = data.ConversationLastUsers,
                    CreatedOn = DateTime.UtcNow,
                });
        }

        private class MessagesServiceData
        {
            private static readonly ICollection<ApplicationUser> Users =
                Enumerable.Range(1, 10).Select(_ => new ApplicationUser()).ToImmutableList();

            public Conversation ConversationFirstUsers => new Conversation { Users = Users.Take(5).ToList() };

            public Conversation ConversationLastUsers => new Conversation { Users = Users.TakeLast(5).ToList() };
        }
    }
}

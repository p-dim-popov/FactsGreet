// ReSharper disable VirtualMemberCallInConstructor
namespace FactsGreet.Data.Models
{
    using System;
    using System.Collections.Generic;

    using FactsGreet.Data.Common.Models;

    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser, IAuditInfo, IDeletableEntity
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Roles = new HashSet<IdentityUserRole<string>>();
            this.Claims = new HashSet<IdentityUserClaim<string>>();
            this.Logins = new HashSet<IdentityUserLogin<string>>();
        }

        // Audit info
        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        // Deletable entity
        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public virtual ICollection<IdentityUserRole<string>> Roles { get; set; }

        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }

        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }

        /*
        /////////////////////////////////////
        Additions to template ApplicationUser
        /////////////////////////////////////
        */

        public virtual ICollection<Notification> Notifications { get; set; }
            = new HashSet<Notification>();

        public virtual ICollection<Notification> Actions { get; set; }
            = new HashSet<Notification>();

        public virtual ICollection<Edit> Edits { get; set; }
            = new HashSet<Edit>();

        public virtual ICollection<Article> Articles { get; set; }
            = new HashSet<Article>();

        public virtual ICollection<ConversationParticipance> ConversationParticipances { get; set; }
            = new HashSet<ConversationParticipance>();

        public virtual ICollection<Follow> Followers { get; set; }
            = new HashSet<Follow>();

        public virtual ICollection<Follow> Followings { get; set; }
            = new HashSet<Follow>();

        public virtual ICollection<Message> SentMessages { get; set; }
            = new HashSet<Message>();

        public virtual ICollection<Report> SentReports { get; set; }
            = new HashSet<Report>();

        public virtual ICollection<Report> ReceivedReports { get; set; }

        public virtual ICollection<Star> FavoriteArticles { get; set; }
    }
}

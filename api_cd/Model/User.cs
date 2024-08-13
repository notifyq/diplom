using System;
using System.Collections.Generic;

namespace api_CodeFlow.Model
{
    public partial class User
    {
        public User()
        {
            Publishers = new HashSet<Publisher>();
            Purchases = new HashSet<Purchase>();
        }

        public int UserId { get; set; }
        public string UserLogin { get; set; } = null!;
        public string UserPassword { get; set; } = null!;
        public string? UserName { get; set; }
        public string UserEmail { get; set; } = null!;
        public string? UserImage { get; set; }
        public int UserRole { get; set; }
        public int UserStatus { get; set; }

        public virtual Role UserRoleNavigation { get; set; } = null!;
        public virtual Status UserStatusNavigation { get; set; } = null!;
        public virtual ICollection<Publisher> Publishers { get; set; }
        public virtual ICollection<Purchase> Purchases { get; set; }
}
    }

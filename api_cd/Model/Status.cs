using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace api_CodeFlow.Model
{
    public partial class Status
    {
        public Status()
        {
            Products = new HashSet<Product>();
            Purchases = new HashSet<Purchase>();
            Users = new HashSet<User>();
        }

        public int StatusId { get; set; }
        public string StatusName { get; set; } = null!;
        public int StatusType { get; set; }

        public virtual StatusType StatusTypeNavigation { get; set; } = null!;
        [JsonIgnore]
        public virtual ICollection<Product> Products { get; set; }
        [JsonIgnore]
        public virtual ICollection<Purchase> Purchases { get; set; }
        [JsonIgnore]
        public virtual ICollection<ProductUpdate> ProductUpdates { get; set; }
        [JsonIgnore]
        public virtual ICollection<User> Users { get; set; }
    }
}

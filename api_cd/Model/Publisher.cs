using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace api_CodeFlow.Model
{
    public partial class Publisher
    {
        public Publisher()
        {
            Products = new HashSet<Product>();
        }

        public int PublisherId { get; set; }
        public string PublisherName { get; set; } = null!;
        public int PublisherUserId { get; set; }

        public virtual User PublisherUser { get; set; } = null!;
        [JsonIgnore]
        public virtual ICollection<Product> Products { get; set; }
    }
}

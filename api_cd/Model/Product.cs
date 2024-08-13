using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace api_CodeFlow.Model
{
    public partial class Product
    {
        public Product()
        {
            ProductGenres = new HashSet<ProductGenre>();
            ProductImages = new HashSet<ProductImage>();
            ProductUpdates = new HashSet<ProductUpdate>();
            PurchaseLists = new HashSet<PurchaseList>();
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string ProductDescription { get; set; } = null!;
        public decimal ProductPrice { get; set; }
        public int ProductPublisherId { get; set; }
        public int ProductStatusId { get; set; }

        public virtual Publisher ProductPublisher { get; set; } = null!;
        public virtual Status ProductStatus { get; set; } = null!;
        public virtual ICollection<ProductGenre> ProductGenres { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
        public virtual ICollection<ProductUpdate> ProductUpdates { get; set; }
        [JsonIgnore]
        public virtual ICollection<PurchaseList> PurchaseLists { get; set; }
    }
}

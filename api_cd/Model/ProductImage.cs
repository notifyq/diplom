using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace api_CodeFlow.Model
{
    public partial class ProductImage
    {
        public int ProductImageId { get; set; }
        public int ProductId { get; set; }
        public string ProductImagePath { get; set; } = null!;
        [JsonIgnore]
        public virtual Product Product { get; set; } = null!;
    }
}

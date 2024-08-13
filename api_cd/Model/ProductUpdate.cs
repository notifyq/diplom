using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace api_CodeFlow.Model
{
    public partial class ProductUpdate
    {
        public int ProductUpdateId { get; set; }
        public int ProductId { get; set; }
        public string ProductVersion { get; set; } = null!;
        public DateTime UpdateDate { get; set; }
        public int UpdateStatus { get; set; }
        
        public virtual Status UpdateStatusNavigation { get; set; }
        [JsonIgnore]
        public virtual Product Product { get; set; } = null!;
    }

    public class ProductUpdateModelSend
    {
        public int ProductUpdateId { get; set; }
        public int ProductId { get; set; }
        public string ProductVersion { get; set; } = null;
        public DateTime UpdateDate { get; set; }
        public int UpdateStatus { get; set; }
        public virtual Status UpdateStatusNavigation { get; set; }
    }
}

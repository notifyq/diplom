using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace api_CodeFlow.Model
{
    public partial class PurchaseList
    {
        public int PurchaseListId { get; set; }
        public int PurchaseId { get; set; }
        public int ProductId { get; set; }
        public decimal ProductSpentMoney { get; set; }
        public string Key { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
        [JsonIgnore]
        public virtual Purchase Purchase { get; set; } = null!;
    }
}

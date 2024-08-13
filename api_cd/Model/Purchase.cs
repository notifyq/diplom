using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace api_CodeFlow.Model
{
    public partial class Purchase
    {
        public Purchase()
        {
            PurchaseLists = new HashSet<PurchaseList>();
        }
        [Key]
        public int PurchasesId { get; set; }
        public int UserId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int PurchaseStatus { get; set; }

        public virtual Status PurchaseStatusNavigation { get; set; } = null!;
        [JsonIgnore]
        public virtual User User { get; set; } = null!;
        public virtual ICollection<PurchaseList> PurchaseLists { get; set; }
    }
}

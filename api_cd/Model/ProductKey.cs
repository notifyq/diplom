using System.Text.Json.Serialization;

namespace api_CodeFlow.Model
{
    public class ProductKey
    {
        public int ProductKeyId { get; set; }
        public int ProductId { get; set; }
        public string Key { get; set; } = null!;
        public int UserId { get; set; }
        [JsonIgnore]
        public virtual Product Product { get; set; } = null!;
        [JsonIgnore]
        public virtual User User { get; set; } = null!;
    }
}

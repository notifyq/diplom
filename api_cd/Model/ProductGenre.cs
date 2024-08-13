using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace api_CodeFlow.Model
{
    public partial class ProductGenre
    {
        public int ProductGenreId { get; set; }
        public int ProductId { get; set; }
        public int GenreId { get; set; }
        public virtual Genre Genre { get; set; } = null!;
        [JsonIgnore]
        public virtual Product Product { get; set; } = null!;
    }
}

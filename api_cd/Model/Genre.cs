using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace api_CodeFlow.Model
{
    public partial class Genre
    {
        public Genre()
        {
            ProductGenres = new HashSet<ProductGenre>();
        }

        public int GenreId { get; set; }
        public string GenreName { get; set; } = null!;
        [JsonIgnore]
        public virtual ICollection<ProductGenre> ProductGenres { get; set; }
    }
}

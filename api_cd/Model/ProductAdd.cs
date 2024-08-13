namespace api_CodeFlow.Model
{
    public class ProductAdd
    {
        public string ProductName { get; set; } = null;
        public string ProductDescription { get; set; } = null;
        public decimal ProductPrice { get; set; }
        public List<int> GenreIds { get; set; }

    }
}

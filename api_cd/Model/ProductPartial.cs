namespace api_CodeFlow.Model
{
    public partial class Product
    {
        public List<string> Images
        {
            get
            {
                string projectDirectory = System.IO.Directory.GetCurrentDirectory();
                if (ProductImages.Count == 0)
                {
                    return null;
                }

                List<string> data = new List<string>();
                foreach (var image in ProductImages)
                {
                    try
                    {
                        // path = "{directory}/publishers/{publisher_name}/{product_name}/images/{image_name}";
                        byte[] imageBytes = File.ReadAllBytes($"{projectDirectory}/publishers/{ProductPublisher.PublisherName}/{ProductName}/images/{image.ProductImagePath}");


                        using (MemoryStream stream = new MemoryStream(imageBytes))
                        {
                            string base64String = Convert.ToBase64String(imageBytes);
                            data.Add(base64String);
                        }
                    }
                    catch (Exception)
                    {

                    }

                }
                return data;
            }
        }
    }
}

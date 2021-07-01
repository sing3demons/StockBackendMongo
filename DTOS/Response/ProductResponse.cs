using StockBackendMongo.Entities;

namespace StockBackendMongo.DTOS.Response
{
    public class ProductResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Stock { get; set; }
        public string Image { get; set; }

        public static ProductResponse FromProduct(Product product)
        {
            return new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Image = "https://localhost:5001/" + "images/" + product.Image,
                Price = product.Price,
                Stock = product.Stock,
            };
        }
    }
}
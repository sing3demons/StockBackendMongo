using StockBackendMongo.Entities;
using StockBackendMongo.Settings;

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
   

        public static ProductResponse FromProduct(Product product,Constants constants)
        {
            return new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Image = constants.Host + constants.Image + product.Image,
                Price = product.Price,
                Stock = product.Stock,
            };
        }
    }
}
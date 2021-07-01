using StockBackendMongo.DTOS.Response;
using StockBackendMongo.Entities;

namespace StockBackendMongo.Settings
{
    public static class Extensions
    {
        public static ProductResponse AsDto(this Product product)
        {
            return new ProductResponse{
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
using System.ComponentModel.DataAnnotations;

namespace StockBackendMongo.DTOS
{
    public class CreateProduct
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Stock { get; set; }
    }
}
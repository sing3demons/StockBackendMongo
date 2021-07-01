using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace StockBackendMongo.DTOS
{
    public class UpdateProduct
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Stock { get; set; }
        public List<IFormFile> FormFile { get; set; }

    }
}
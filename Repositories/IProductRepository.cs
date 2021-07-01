using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StockBackendMongo.Entities;

namespace StockBackendMongo.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> FindAll();
        Task<Product> FindOne(string id);
        Task Create(Product product);
        Task Update(Product product);
        Task Delete(string id);

        Task<(string errorMessage, string imageName)> UploadImage(List<IFormFile> formFiles);
    }
}
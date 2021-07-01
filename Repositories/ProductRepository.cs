using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using StockBackendMongo.Entities;

namespace StockBackendMongo.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private const string databaseName = "stock_backend";
        private const string collectionName = "products";
        private readonly IMongoCollection<Product> collection;
        private readonly FilterDefinitionBuilder<Product> filterBuilder = Builders<Product>.Filter;
        private readonly IUploadFileService uploadFileService;

        public ProductRepository(IMongoClient client, IUploadFileService uploadFileService)
        {
            var database = client.GetDatabase(databaseName);
            collection = database.GetCollection<Product>(collectionName);
            this.uploadFileService = uploadFileService;
        }
        public async Task Create(Product product)
        {
            await collection.InsertOneAsync(product);
        }

        public async Task Delete(string id)
        {
            var filter = filterBuilder.Eq(p => p.Id, id);
            await collection.DeleteOneAsync(filter);
        }

        public async Task<IEnumerable<Product>> FindAll()
        {
            var documents = await collection.Find(new BsonDocument()).ToListAsync();
            return documents;
        }

        public async Task<Product> FindOne(string id)
        {
            var filter = filterBuilder.Eq(p => p.Id, id);
            var document = await collection.Find(filter).FirstOrDefaultAsync();
            return document;
        }

        public async Task Update(Product product)
        {
            var filter = filterBuilder.Eq(p => p.Id, product.Id);
            await collection.ReplaceOneAsync(filter, product);
        }

        public async Task<(string errorMessage, string imageName)> UploadImage(List<IFormFile> formFiles)
        {
            string errorMessage = string.Empty;
            string imageName = string.Empty;
            if (uploadFileService.IsUpload(formFiles))
            {
                errorMessage = uploadFileService.Validation(formFiles);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    imageName = (await uploadFileService.UploadImages(formFiles))[0];
                }
            }
            return (errorMessage, imageName);
        }
    }
}
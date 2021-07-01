using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using StockBackendMongo.DTOS;
using StockBackendMongo.DTOS.Response;
using StockBackendMongo.Entities;
using StockBackendMongo.Repositories;
using StockBackendMongo.Settings;
//using StockBackendMongo.Models;

namespace StockBackendMongo.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository repository;
        private readonly IUploadFileService uploadFileService;

        public ProductsController(IProductRepository repository, IUploadFileService uploadFileService)
        {
            this.repository = repository;
            this.uploadFileService = uploadFileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            // var products = (await repository.FindAll()).Select(ProductResponse.FromProduct);
            var products = (await repository.FindAll()).Select(p => p.AsDto());

            return Ok(new { products = products });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponse>> GetProductById(string id)
        {
            var product = await repository.FindOne(id);
            if (product is null) return NotFound();

            return Ok(new { product = product.AsDto() });
        }

        [HttpPost("")]
        public async Task<IActionResult> PostProduct([FromForm] CreateProduct request)
        {

            (string errorMessage, string imageName) = await repository.UploadImage(request.FormFile);

            if (!string.IsNullOrEmpty(errorMessage)) return BadRequest(error: errorMessage);


            var product = request.Adapt<Product>();
            product.Image = imageName;

            await repository.Create(product);
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        private Exception Exception(string errorMessage)
        {
            throw new NotImplementedException();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(string id, [FromForm] UpdateProduct request)
        {
            var product = await repository.FindOne(id);
            if (product == null) return NotFound();

            (string errorMessage, string imageName) = await repository.UploadImage(request.FormFile);


            if (!string.IsNullOrEmpty(errorMessage)) return BadRequest(error: errorMessage);

            if (!string.IsNullOrEmpty(imageName))
            {
                await uploadFileService.RemoveImage(product.Image);
                product.Image = imageName;
            }


            var result = request.Adapt(product);

            await repository.Update(result);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProductById(string id)
        {
            var product = await repository.FindOne(id);
            if (product == null) return NotFound();

            if (!string.IsNullOrEmpty(product.Image))
            {
                await uploadFileService.RemoveImage(product.Image);
            }

            await repository.Delete(product.Id);

            return NoContent();
        }
    }
}
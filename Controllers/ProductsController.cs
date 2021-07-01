using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using StockBackendMongo.DTOS;
using StockBackendMongo.Entities;
using StockBackendMongo.Repositories;
//using StockBackendMongo.Models;

namespace StockBackendMongo.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository repository;

        public ProductsController(IProductRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetProducts()
        {
            var products = await repository.FindAll();

            return Ok(new { products = products });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(string id)
        {
            var product = await repository.FindOne(id);
            if (product == null) return NotFound();

            return Ok(new { product = product });
        }

        [HttpPost("")]
        public async Task<IActionResult> PostProduct([FromForm] CreateProduct request)
        {
            var product = request.Adapt<Product>();
            await repository.Create(product);
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(string id, [FromForm] UpdateProduct request)
        {
            var product = await repository.FindOne(id);
            if (product == null) return NotFound();

            var result = request.Adapt(product);

            Console.WriteLine(request);

            await repository.Update(result);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProductById(string id)
        {
            var product = await repository.FindOne(id);
            if (product == null) return NotFound();

            await repository.Delete(product.Id);

            return NoContent();
        }
    }
}
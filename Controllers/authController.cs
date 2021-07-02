using System;
using BC = BCrypt.Net.BCrypt;
using System.Net;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using StockBackendMongo.DTOS.Request;
using StockBackendMongo.Entities;
using StockBackendMongo.Repositories;



//using StockBackendMongo.Models;

namespace StockBackendMongo.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class authController : ControllerBase
    {
        private readonly IAuthRepository repository;

        public authController(IAuthRepository repository)
        {
            this.repository = repository;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            string passwordHash = BC.HashPassword(request.Password);
            var user = request.Adapt<User>();
            user.Password = passwordHash;
            await repository.Register(user);
            return StatusCode((int)HttpStatusCode.Created);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<User>> Login([FromBody] LoginRequest request)
        {
            var user = await repository.Login(request.Email, request.Password);
            var token = repository.GenerateToken(user);
            return Ok(new { token = token });
        }

        // 
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            // var token =  HttpContext.GetTokenAsync("access_token");
            
            // string accessToken = httpContext.Request.Headers["Authorization"];
            
            string authHeader = HttpContext.Request.Headers["Authorization"];
            if (authHeader == null) throw new Exception("The authorization header is either empty or isn't Basic."); ;

            var user = repository.GetInfo(authHeader);

            return Ok(new
            {
                id = user
            });
        }

    }
}

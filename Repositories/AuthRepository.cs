using System.Linq;
using System.Text;
using System.Security.Claims;
using System.Threading.Tasks;
using MongoDB.Driver;
using BC = BCrypt.Net.BCrypt;
using StockBackendMongo.Entities;
using System;
using System.IdentityModel.Tokens.Jwt;
using StockBackendMongo.Settings;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace StockBackendMongo.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly JwtSettings jwtSettings;

        private const string databaseName = "stock_backend";
        private const string collectionName = "user";
        private readonly IMongoCollection<User> collection;
        private readonly FilterDefinitionBuilder<User> filterBuilder = Builders<User>.Filter;
        public AuthRepository(IMongoClient client, JwtSettings jwtSettings)
        {
            var database = client.GetDatabase(databaseName);
            collection = database.GetCollection<User>(collectionName);
            this.jwtSettings = jwtSettings;
        }

        public async Task<User> Login(string email, string password)
        {
            var filter = filterBuilder.Eq(u => u.Email, email);
            var user = await collection.Find(filter).FirstOrDefaultAsync();

            if (user is null || !BC.Verify(password, user.Password))
            {
                throw new Exception("Email or Password Incorrect");
            }
            return user;


        }


        public async Task Register(User user)
        {
            await collection.InsertOneAsync(user);
        }

        public string GenerateToken(User user)
        {
            var cliams = new[]{
                new Claim(JwtRegisteredClaimNames.Sub,user.Id),
                new Claim("name", user.Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
            return BuildToken(cliams);
        }

        private string BuildToken(Claim[] cliams)
        {
            var expires = DateTime.Now.AddDays(Convert.ToDouble(jwtSettings.Expire));
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: cliams,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Object GetInfo(string authHeader)
        {
            //Extract credentials
            string accessToken = authHeader.Substring("Bearer ".Length).Trim();

            var token = new JwtSecurityTokenHandler().ReadJwtToken(accessToken) as JwtSecurityToken;
            Console.WriteLine("token: " + token);
            var id = token.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Sub).Value;
            var name = token.Claims.First(claim => claim.Type == "name").Value;
            var exp = token.Claims.First(c => c.Type == "exp").Value;


            return new { sub = id, name = name, exp = exp };
        }
    }
}
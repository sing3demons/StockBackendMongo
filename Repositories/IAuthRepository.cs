using System;
using System.Threading.Tasks;
using StockBackendMongo.Entities;

namespace StockBackendMongo.Repositories
{
    public interface IAuthRepository
    {
        Task Register(User user);
        Task<User> Login(string email, string password);

        string GenerateToken(User user);

        Object GetInfo(string authHeader);
    }
}
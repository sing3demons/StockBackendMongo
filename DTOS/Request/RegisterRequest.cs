using System.ComponentModel.DataAnnotations;

namespace StockBackendMongo.DTOS.Request
{
    public class RegisterRequest
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
    }
}
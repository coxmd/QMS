using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using QueueManagementSystem.MVC.Data;
using QueueManagementSystem.MVC.Models;

namespace QueueManagementSystem.MVC.Services
{
    public class AuthService : IAuthService
    {
        private readonly QueueManagementSystemContext _context;

        public AuthService(QueueManagementSystemContext context)
        {
            _context = context;
        }

        public async Task<Models.ServiceProvider?> Authenticate(LoginModel user)
        {
            Models.ServiceProvider? provider = await _context.ServiceProviders.SingleOrDefaultAsync(sp => sp.Email == user.Email);

            if (provider == null || !BCrypt.Net.BCrypt.Verify(user.Password, provider.Password))
            {
                return null;
            }

            return provider;
        }
    }
}
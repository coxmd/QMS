using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using QueueManagementSystem.MVC.Data;
using QueueManagementSystem.MVC.Models;
using QueueManagementSystem.MVC.Models.Users;

namespace QueueManagementSystem.MVC.Services
{
    public class AuthService : IAuthService
    {
        private readonly QueueManagementSystemContext _context;

        public AuthService(QueueManagementSystemContext context)
        {
            _context = context;
        }

        public async Task<SystemUsersModel?> Authenticate(LoginModel user)
        {
            SystemUsersModel? provider = await _context.SystemUsers
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Username == user.UserName);

            if (provider == null || !BCrypt.Net.BCrypt.Verify(user.Password, provider.Password))
            {
                return null;
            }

            return provider;
        }
    }
}
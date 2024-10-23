using QueueManagementSystem.MVC.Models;

namespace QueueManagementSystem.MVC.Services
{
    public interface IAuthService
    {
        Task<Models.ServiceProvider>? Authenticate(LoginModel user);
    }
}
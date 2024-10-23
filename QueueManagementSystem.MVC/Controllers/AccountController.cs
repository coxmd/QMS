using System.Security.Claims;
using Enyim.Caching.Memcached;
using FastReport.AdvMatrix;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QueueManagementSystem.MVC.Data;
using QueueManagementSystem.MVC.Models;
using QueueManagementSystem.MVC.Services;

namespace Queue_Management_System.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IDbContextFactory<QueueManagementSystemContext> _dbFactory;

        public AccountController(IAuthService authService, IDbContextFactory<QueueManagementSystemContext> dbFactory)
        {
            _authService = authService;
            _dbFactory = dbFactory;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordModel change)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            using var context = _dbFactory.CreateDbContext();
            var user = context.ServiceProviders.FirstOrDefault(s => s.Email == change.UserEmail);
            if (user == null)
            {
                // Handle user not found case
                ModelState.AddModelError(string.Empty, "Email not Registered ");
                return View();
            }
            else
            {
                
                if (!BCrypt.Net.BCrypt.Verify(change.OldPassword, user.Password))
                {
                    
                    
                    ModelState.AddModelError(string.Empty, "Wrong Old password");
                    return View(change);

                }
                else
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(change.NewPassword);
                    context.Update(user);
                    context.SaveChanges();
                    TempData["SuccessMessage"] = "Your password has been changed successfully.";
                    return View();

                }

            }
            }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel user, string returnUrl=null)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            using var context = _dbFactory.CreateDbContext();
            var userEmail = context.ServiceProviders.FirstOrDefault(s => s.Email == user.Email);
             if(userEmail == null)
            {
                // Handle user not found case
                ModelState.AddModelError(string.Empty, "Email not Registered ");
                return View();
            }

            var serviceProvider = await _authService.Authenticate(user)!;
            if (serviceProvider == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt");
                return View();
            }

            string? role = serviceProvider.IsAdmin ? "Admin" : "";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, serviceProvider.Id.ToString()),
                new Claim(ClaimTypes.Name, serviceProvider.FullNames!),
                new Claim(ClaimTypes.Email, serviceProvider.Email!),
                new Claim(ClaimTypes.Role, role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "MyCookieScheme");

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(10)
            };

            await HttpContext.SignInAsync("MyCookieScheme", new ClaimsPrincipal(claimsIdentity), authProperties);

            if (returnUrl != null)
            {
               return LocalRedirect(returnUrl); 
            }
            else return RedirectToAction("Index", "Home");
                   
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieScheme");
            return RedirectToAction("Index", "Home");
        }
    }
}
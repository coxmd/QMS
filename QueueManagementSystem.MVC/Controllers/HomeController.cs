using System.Diagnostics;
using System.Security.Claims;
using BlazorBootstrap;
using Microsoft.AspNetCore.Mvc;
using QueueManagementSystem.MVC.Models;

namespace QueueManagementSystem.MVC.Controllers;

public class HomeController : Controller
{
    private readonly ToastService _toastService;
    private readonly ILogger<HomeController> _logger;
    //private readonly IMemcachedClient _memcachedClient;

    public HomeController(ILogger<HomeController> logger, ToastService toastService)
    {
        _logger = logger;
        _toastService = toastService;
    }

    public IActionResult Index()
    {
        // Get user's role from claims
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var privileges = User.Claims
            .Where(c => c.Type == "Privilege")
            .Select(c => c.Value)
            .ToList();

        ViewData["UserRole"] = userRole;
        ViewData["Privileges"] = privileges;
        ViewData["BreadcrumbTitle"] = "Home";

        return View();
    }

    [Route("/Home/AccessDenied")]
    public IActionResult AccessDenied()
    {
        Response.StatusCode = 403;
        _toastService.Notify(new ToastMessage
        {
            Type = ToastType.Danger,
            Title = "ACCESS DENIED",
            HelpText = $"{DateTime.Now}",
            Message = "You don't have sufficient privileges to perform this action."
        });
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Feedback()
    {
        ViewData["BreadcrumbTitle"] = "Feedback";
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

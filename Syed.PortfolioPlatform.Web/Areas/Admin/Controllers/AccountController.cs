using Microsoft.AspNetCore.Mvc;
using Syed.PortfolioPlatform.Application.Auth.DTOs;
using Syed.PortfolioPlatform.Web.Models;

[Area("Admin")]
public class AccountController : Controller
{
    // Test Build
    private readonly IHttpClientFactory _httpClientFactory;

    public AccountController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View("~/Areas/Admin/Views/Account/Login.cshtml", new LoginViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var client = _httpClientFactory.CreateClient("api");
        
        try
        {
            var response = await client.PostAsJsonAsync("api/auth/login", new
            {
                username = model.Username,
                password = model.Password
            });

            if (!response.IsSuccessStatusCode)
            {
                model.ErrorMessage = "Invalid username or password";
                return View(model);
            }

            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

            // Store JWT tokens in session
            HttpContext.Session.SetString("AccessToken", authResponse!.AccessToken);
            HttpContext.Session.SetString("RefreshToken", authResponse.RefreshToken);

            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }
        catch (Exception ex)
        {
            model.ErrorMessage = ex.Message;
            return View(model);
        }
    }

    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Account", new { area = "Admin" });
    }
}

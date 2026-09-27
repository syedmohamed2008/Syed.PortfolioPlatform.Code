using Microsoft.AspNetCore.Mvc;
using Syed.PortfolioPlatform.Application.Auth.DTOs;
using Syed.PortfolioPlatform.Application.DTOs.Articles;
using System.Net.Http.Headers;

[Area("Admin")]
public class DashboardController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public DashboardController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index()
    {
        var client = _httpClientFactory.CreateClient("api");

        var accessToken = HttpContext.Session.GetString("AccessToken");
        if (string.IsNullOrEmpty(accessToken))
            return RedirectToAction("Login", "Account");

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        // Call protected API (example: get articles)
        var response = await client.GetAsync("/api/admin/articles");
        if (!response.IsSuccessStatusCode)
        {
            // Token might be expired — refresh token flow
            var refreshToken = HttpContext.Session.GetString("RefreshToken");
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var refreshResponse = await client.PostAsJsonAsync("api/auth/refresh", new
                {
                    RefreshToken = refreshToken
                });

                if (refreshResponse.IsSuccessStatusCode)
                {
                    var authResponse = await refreshResponse.Content.ReadFromJsonAsync<AuthResponse>();
                    HttpContext.Session.SetString("AccessToken", authResponse!.AccessToken);
                    HttpContext.Session.SetString("RefreshToken", authResponse.RefreshToken);

                    // Retry the request
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", authResponse.AccessToken);

                    response = await client.GetAsync("/api/admin/articles");
                }
            }
        }

        var articles = await response.Content.ReadFromJsonAsync<List<ArticleDto>>();

        return View(articles);
    }
}

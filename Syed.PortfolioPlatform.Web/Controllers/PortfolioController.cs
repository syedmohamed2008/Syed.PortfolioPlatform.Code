using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Syed.PortfolioPlatform.Application.DTOs.Articles;
using Syed.PortfolioPlatform.Application.DTOs.ContactMessage;
using Syed.PortfolioPlatform.Web.Configurations;
using Syed.PortfolioPlatform.Web.Controllers;
using Syed.PortfolioPlatform.Web.Models;
using System.Net.Http.Json;

[EnableRateLimiting("fixed")]
public class PortfolioController : BaseController
{
    private readonly HttpClient _httpClient;
    private readonly PortfolioSettings _settings;

    public PortfolioController(
        IHttpClientFactory httpClientFactory,
        PortfolioSettings settings) : base(settings)
    {
        _httpClient = httpClientFactory.CreateClient("api");
        _settings = settings;
    }

    public async Task<IActionResult> Index()
    {
        var latestArticles = await _httpClient.GetFromJsonAsync<List<ArticleDto>>(
            "api/articles/latest?count=3");

        var vm = new HomeViewModel
        {
            LatestArticles = latestArticles ?? new List<ArticleDto>()
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(ContactMessageRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/contact/messages", request);

        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "Message received successfully. I'll get back to you soon.";
        }
        else
        {
            TempData["Error"] = "Something went wrong. Please try again.";
        }

        return RedirectToAction("Index");
    }
}
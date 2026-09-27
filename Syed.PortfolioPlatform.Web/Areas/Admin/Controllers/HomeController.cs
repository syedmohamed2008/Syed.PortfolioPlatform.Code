using Microsoft.AspNetCore.Mvc;
using Syed.PortfolioPlatform.Application.DTOs.Articles;
using Syed.PortfolioPlatform.Web.Models;
using System.Net.Http.Json;

namespace Syed.PortfolioPlatform.Web.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("api");
        }

        public async Task<IActionResult> Index()
        {
            var articles = await _httpClient.GetFromJsonAsync<List<ArticleDto>>(
                "api/articles/latest");

            var vm = new HomeViewModel
            {
                LatestArticles = articles
            };

            return View(vm);
        }
    }
}
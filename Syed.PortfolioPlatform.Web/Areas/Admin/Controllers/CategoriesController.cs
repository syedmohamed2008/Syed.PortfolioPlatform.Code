using Microsoft.AspNetCore.Mvc;
using Syed.PortfolioPlatform.Application.DTOs.Categories;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

[Area("Admin")]
public class CategoriesController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CategoriesController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient CreateClient()
    {
        var client = _httpClientFactory.CreateClient("api");

        var token = HttpContext.Session.GetString("AccessToken");

        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }

    // ==============================
    // GET: Categories
    // ==============================
    [HttpGet]
    public async Task<IActionResult> Index()
    {

        var client = CreateClient();
        var Categories = await client.GetFromJsonAsync<List<CategoryDto>>("api/admin/Categories");

        return View(Categories ?? new List<CategoryDto>());
    }

    // ==============================
    // GET: Create
    // ==============================
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // ==============================
    // POST: Create
    // ==============================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCategoryRequest model)
    {
        model.Slug = GenerateSlug(model.Name);

        if (!ModelState.IsValid)
            return View(model);

        var client = CreateClient();
        var response = await client.PostAsJsonAsync("api/admin/Categories", model);

        if (response.IsSuccessStatusCode)
            return RedirectToAction("Index");

        
        var errorMessage = await response.Content.ReadAsStringAsync();

        ModelState.AddModelError("Name", errorMessage);

        return View(model);
    }

    private string GenerateSlug(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        // Convert to lowercase
        value = value.ToLowerInvariant();

        // Remove accents (é → e)
        value = RemoveDiacritics(value);

        // Remove invalid characters
        value = Regex.Replace(value, @"[^a-z0-9\s-]", "");

        // Replace multiple spaces with single space
        value = Regex.Replace(value, @"\s+", " ").Trim();

        // Replace spaces with hyphens
        value = value.Replace(" ", "-");

        return value;
    }

    private string RemoveDiacritics(string text)
    {
        var normalized = text.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();

        foreach (var c in normalized)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(c);
            }
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }


    // ==============================
    // GET: Edit
    // ==============================
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var client = CreateClient();
        var response = await client.GetAsync($"api/admin/Categories/{id}");

        if (!response.IsSuccessStatusCode)
            return NotFound();

        var category = await response.Content
            .ReadFromJsonAsync<UpdateCategoryRequest>();

        return View(category);
    }

    // ==============================
    // POST: Edit
    // ==============================
    [HttpPost]
    public async Task<IActionResult> Edit(UpdateCategoryRequest model)
    {
        model.Slug = GenerateSlug(model.Name);

        if (!ModelState.IsValid)
            return View(model);

        var client = CreateClient();

        var response = await client.PutAsJsonAsync($"api/admin/Categories/{model.Id}", model);

        if (response.IsSuccessStatusCode)
            return RedirectToAction("Index");

        var errorMessage = await response.Content.ReadAsStringAsync();

        ModelState.AddModelError("Name", errorMessage);

        return View(model);
    }

    // ==============================
    // GET: Delete
    // ==============================
    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var client = CreateClient();
        await client.DeleteAsync($"api/admin/Categories/{id}");

        return RedirectToAction("Index");
    }

   

}

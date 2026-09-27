using Microsoft.AspNetCore.Mvc;
using Syed.PortfolioPlatform.Application.DTOs.Categories;
using Syed.PortfolioPlatform.Application.DTOs.Tags;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

[Area("Admin")]
public class TagsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public TagsController(IHttpClientFactory httpClientFactory)
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
    // GET: Tags
    // ==============================
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var client = CreateClient();
        var tags = await client.GetFromJsonAsync<List<TagDto>>("api/admin/tags");

        return View(tags ?? new List<TagDto>());
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
    public async Task<IActionResult> Create(CreateTagRequest model)
    {
        model.Slug = GenerateSlug(model.Name);

        if (!ModelState.IsValid)
            return View(model);

        var client = CreateClient();
        var response = await client.PostAsJsonAsync("api/admin/Tags", model);

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
        var response = await client.GetAsync($"api/admin/Tags/{id}");

        if (!response.IsSuccessStatusCode)
            return NotFound();

        var tag = await response.Content
            .ReadFromJsonAsync<UpdateTagRequest>();

        return View(tag);
    }

    // ==============================
    // POST: Edit
    // ==============================
    [HttpPost]
    public async Task<IActionResult> Edit(UpdateTagRequest model)
    {
        model.Slug = GenerateSlug(model.Name);

        if (!ModelState.IsValid)
            return View(model);

        var client = CreateClient();
        var response = await client.PutAsJsonAsync($"api/admin/tags/{model.Id}", model);

        var errorContent = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
            return RedirectToAction("Index");

        ModelState.AddModelError("Name", "Duplicate tag name.");
        
        return View(model);
    }

    // ==============================
    // GET: Delete
    // ==============================
    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var client = CreateClient();
        await client.DeleteAsync($"api/admin/tags/{id}");

        return RedirectToAction("Index");
    }

   

}

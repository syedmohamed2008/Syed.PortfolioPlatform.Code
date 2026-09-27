using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Syed.PortfolioPlatform.Application.DTOs.Articles;
using Syed.PortfolioPlatform.Application.DTOs.Categories;
using Syed.PortfolioPlatform.Application.DTOs.Tags;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;


[Area("Admin")]
public class ArticlesController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;


    public ArticlesController(IHttpClientFactory httpClientFactory, IWebHostEnvironment env)
    {
        _httpClientFactory = httpClientFactory;
    }

    // Article controller changes
    private HttpClient CreateClient()
    {
        var client = _httpClientFactory.CreateClient("api");

        // Get JWT token from session
        var token = HttpContext.Session.GetString("AccessToken");
        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }


    // GET: Articles
    public async Task<IActionResult> Index()
    {
        var client = CreateClient();
        var articles = await client.GetFromJsonAsync<List<ArticleDto>>("api/admin/articles");
        return View(articles ?? new List<ArticleDto>());
    }

    // GET: Create
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var client = CreateClient();

        var categories = await client.GetFromJsonAsync<List<CategoryDto>>("api/admin/categories");
        var tags = await client.GetFromJsonAsync<List<TagDto>>("api/admin/tags");

        var model = new CreateArticleRequest
        {
            CategoryOptions = categories?.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }) ?? Enumerable.Empty<SelectListItem>(),

            TagOptions = tags?.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Name
            }) ?? Enumerable.Empty<SelectListItem>()
        };

        return View(model);
    }

    // POST: Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateArticleRequest model)
    {
        if (!ModelState.IsValid)
        {
            await ReloadDropdowns(model);
            
        }

        var request = new CreateArticleRequest
        {
            Title = model.Title,
            Content = model.Content,
            CategoryId = model.CategoryId,
            TagIds = model.TagIds,
            Slug = GenerateSlug(model.Title),
            IsPublished = model.IsPublished, 
            YouTubeLink = model.YouTubeLink,
            PDFLink = model.PDFLink,
        };

        var client = CreateClient();

        if (model.FeaturedImage != null && model.FeaturedImage.Length > 0)
        {
            using var filecontent = new MultipartFormDataContent();

            filecontent.Add(new StreamContent(model.FeaturedImage.OpenReadStream()),
                        "file",
                        model.FeaturedImage.FileName);

            var uploadResponse = await client.PostAsync("api/upload", filecontent);

            //uploadResponse.EnsureSuccessStatusCode();

            if (!uploadResponse.IsSuccessStatusCode)
            {
                var error = await uploadResponse.Content.ReadAsStringAsync();
                throw new Exception($"Upload failed: {error}");
            }

            var imageUrl = await uploadResponse.Content.ReadAsStringAsync();

            request.ImageUrl = imageUrl.Trim('"');
        }

        var response = await client.PostAsJsonAsync("api/admin/articles", request);

        if (response.IsSuccessStatusCode)
            return RedirectToAction("Index", "Articles", new { area = "Admin" });

        var errorContent = await response.Content.ReadAsStringAsync();
        ModelState.AddModelError("", $"Failed to create article: {errorContent}");

        await ReloadDropdowns(model);
        return View(model);
    }

    private async Task ReloadDropdowns(CreateArticleRequest model)
    {
        var client = CreateClient();

        var categories = await client.GetFromJsonAsync<List<CategoryDto>>("api/admin/categories");
        var tags = await client.GetFromJsonAsync<List<TagDto>>("api/admin/tags");

        model.CategoryOptions = categories?.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name
        }) ?? Enumerable.Empty<SelectListItem>();

        model.TagOptions = tags?.Select(t => new SelectListItem
        {
            Value = t.Id.ToString(),
            Text = t.Name
        }) ?? Enumerable.Empty<SelectListItem>();
    }


    private string GenerateSlug(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        // Convert to lowercase
        value = value.ToLowerInvariant();

        value = value.Replace("#", "sharp");

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
        var article = await client.GetFromJsonAsync<ArticleDto>($"api/admin/articles/{id}");

        if (article == null)
            return NotFound();

        // Load categories and tags
        var categories = await client.GetFromJsonAsync<List<CategoryDto>>("api/admin/categories");
        var tags = await client.GetFromJsonAsync<List<TagDto>>("api/admin/tags");

        var model = new UpdateArticleRequest
        {
            Id = article.Id,
            Title = article.Title,
            Content = article.Content,
            CategoryId = article.CategoryId,
            TagIds = article.TagIds ?? new List<Guid>(),
            IsPublished = article.IsPublished,
            YouTubeLink =  article.YouTubeLink,
            PDFLink = article.PDFLink,

            CategoryOptions = categories?.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,
                Selected = c.Id == article.CategoryId
            }) ?? Enumerable.Empty<SelectListItem>(),

            TagOptions = tags?.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Name
            }) ?? Enumerable.Empty<SelectListItem>()
        };

        return View(model);
    }

    // ==============================
    // POST: Edit
    // ==============================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateArticleRequest model)
    {
        if (!ModelState.IsValid)
        {
            // Repopulate dropdowns
            var clientReload = CreateClient();
            var categories = await clientReload.GetFromJsonAsync<List<CategoryDto>>("api/admin/categories");
            var tags = await clientReload.GetFromJsonAsync<List<TagDto>>("api/admin/tags");

            model.CategoryOptions = categories?.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }) ?? Enumerable.Empty<SelectListItem>();

            model.TagOptions = tags?.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Name
            }) ?? Enumerable.Empty<SelectListItem>();

            if (model.FeaturedImage != null && model.FeaturedImage.Length > 0)
            {
                using var filecontent = new MultipartFormDataContent();

                filecontent.Add(new StreamContent(model.FeaturedImage.OpenReadStream()),
                            "file",
                            model.FeaturedImage.FileName);

                var uploadResponse = await clientReload.PostAsync("api/upload", filecontent);

                //uploadResponse.EnsureSuccessStatusCode();

                if (!uploadResponse.IsSuccessStatusCode)
                {
                    var error = await uploadResponse.Content.ReadAsStringAsync();
                    throw new Exception($"Upload failed: {error}");
                }

                var imageUrl = await uploadResponse.Content.ReadAsStringAsync();

                model.ImageUrl = imageUrl.Trim('"');
            }

        }

        var request = new UpdateArticleRequest
        {
            Title = model.Title,
            Content = model.Content,
            CategoryId = model.CategoryId,
            TagIds = model.TagIds,
            Slug = GenerateSlug(model.Title),
            ImageUrl = model.ImageUrl,
            IsPublished = model.IsPublished, 
            YouTubeLink = model.YouTubeLink
        };

        var client = CreateClient();
        var response = await client.PutAsJsonAsync($"api/admin/articles/{model.Id}", request);

        if (model.FeaturedImage != null && model.FeaturedImage.Length > 0)
        {
            using var filecontent = new MultipartFormDataContent();

            filecontent.Add(new StreamContent(model.FeaturedImage.OpenReadStream()),
                        "file",
                        model.FeaturedImage.FileName);

            var uploadResponse = await client.PostAsync("api/upload", filecontent);

            //uploadResponse.EnsureSuccessStatusCode();

            if (!uploadResponse.IsSuccessStatusCode)
            {
                var error = await uploadResponse.Content.ReadAsStringAsync();
                throw new Exception($"Upload failed: {error}");
            }

            var imageUrl = await uploadResponse.Content.ReadAsStringAsync();

            request.ImageUrl = imageUrl.Trim('"');
        }

        if (response.IsSuccessStatusCode)
            return RedirectToAction("Index", "Articles", new { area = "Admin" });


        

        var errorContent = await response.Content.ReadAsStringAsync();
        ModelState.AddModelError("", $"Failed to update article: {errorContent}");
        return View(model);
    }

    // ==============================
    // GET: Delete
    // ==============================
    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var client = CreateClient();
        await client.DeleteAsync($"api/admin/articles/{id}");
        return RedirectToAction("Index", "Articles", new { area = "Admin" });
    }
}

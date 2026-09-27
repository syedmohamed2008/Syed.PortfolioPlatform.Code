using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Syed.PortfolioPlatform.Application.DTOs.Articles;
using Syed.PortfolioPlatform.Application.Interfaces;

namespace Syed.PortfolioPlatform.API.Controllers
{
    // Test Build
    [ApiController]
    //[Route("api/[controller]")]
    [Route("api/admin/articles")]
    [Authorize(Roles = "Admin")] // Only Admin can access
    [EnableRateLimiting("fixed")]
    public class AdminArticlesController : ControllerBase
    {
        private readonly IArticleService _articleService;
        private readonly IWebHostEnvironment _env;

        public AdminArticlesController(IArticleService articleService, IWebHostEnvironment env)
        {
            _articleService = articleService;
            _env = env;
        }   

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateArticleRequest request)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("sub")?.Value
                     ?? User.FindFirst("id")?.Value;
            // Extract Guid from JWT claim "id"
           // var userIdClaim = User.FindFirst("id")?.Value;

            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"Claim type: {claim.Type}, Claim value: {claim.Value}");
            }

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid user");

            var article = await _articleService.CreateAsync(request, userId);
            return CreatedAtAction(nameof(Get), new { id = article.Id }, article);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateArticleRequest request)
        {
            var article = await _articleService.UpdateAsync(id, request);
            return Ok(article);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _articleService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var article = await _articleService.GetByIdAsync(id);
            return Ok(article);
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var articles = await _articleService.GetAllAsync();
            return Ok(articles);
        }

       

    }
}

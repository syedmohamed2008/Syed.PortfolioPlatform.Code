using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syed.PortfolioPlatform.Application.DTOs.Categories;
using Syed.PortfolioPlatform.Application.Interfaces;

namespace Syed.PortfolioPlatform.API.Controllers
{
    [Route("api/admin/categories")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
        {
            var result = await _categoryService.CreateAsync(request);

            if (result == null)
            {
                return BadRequest("Category already exists.");
            }

            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequest request)
        {
            var result = await _categoryService.UpdateAsync(id, request);

            if (result == null)
                return BadRequest("Category already exists or not found.");

            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _categoryService.DeleteAsync(id);
            return NoContent();
        }
    }
}

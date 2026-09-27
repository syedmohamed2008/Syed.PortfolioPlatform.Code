using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syed.PortfolioPlatform.Application.DTOs.Categories;
using Syed.PortfolioPlatform.Application.Interfaces;

namespace Syed.PortfolioPlatform.API.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class AdminCategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public AdminCategoryController(ICategoryService categoryService)
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
    }
}

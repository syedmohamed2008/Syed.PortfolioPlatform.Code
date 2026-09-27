using Microsoft.AspNetCore.Mvc;
using Syed.PortfolioPlatform.Application.Interfaces;

namespace Syed.PortfolioPlatform.API.Controllers
{
    [Route("api/tags")]
    [ApiController]
    public class AdminTagController : ControllerBase
    {
        private readonly ITagService _tagService;

        public AdminTagController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tags = await _tagService.GetAllAsync();
            return Ok(tags);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var tag = await _tagService.GetByIdAsync(id);
            return Ok(tag);
        }
    }
}

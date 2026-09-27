using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syed.PortfolioPlatform.Application.DTOs.Tags;
using Syed.PortfolioPlatform.Application.Interfaces;

namespace Syed.PortfolioPlatform.API.Controllers
{
    [Route("api/admin/tags")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagController(ITagService tagService)
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTagRequest request)
        {
            var result = await _tagService.CreateAsync(request);

            if (result == null)
            {
                return BadRequest("Tag already exists.");
            }

            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTagRequest request)
        {
            var result = await _tagService.UpdateAsync(id, request);

            if (result == null)
                return BadRequest("Tag already exists.");

            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _tagService.DeleteAsync(id);
            return NoContent();
        }
    }
}

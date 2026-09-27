using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Syed.PortfolioPlatform.Application.DTOs.ContactMessage;
using Syed.PortfolioPlatform.Application.Interfaces;

namespace Syed.PortfolioPlatform.API.Controllers
{
    [ApiController]
    [Route("api/contact")]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [EnableRateLimiting("fixed")]
        [HttpPost("messages")]
        public async Task<IActionResult> SaveMessage(ContactMessageRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _contactService.SaveMessageAsync(request);

            return Ok(new { message = "Message saved successfully" });
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Syed.PortfolioPlatform.Application.Interfaces;

namespace Syed.PortfolioPlatform.API.Controllers
{
    [Route("api/upload")]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IBlobService _blobService;

        public UploadController(IWebHostEnvironment env, IBlobService blobService)
        {
            _env = env;
            _blobService = blobService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file");

            var imageUrl = await _blobService.UploadAsync(file);



            return Ok(imageUrl);


            // Upload files in wwwroot/images/articles folder. 

            //if (file == null || file.Length == 0)
            //    return BadRequest("No file");

            //var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

            //var folderPath = Path.Combine(_env.WebRootPath, "images", "articles");

            //if (!Directory.Exists(folderPath))
            //{
            //    Directory.CreateDirectory(folderPath);
            //}

            //var filePath = Path.Combine(folderPath, fileName);

            //using (var stream = new FileStream(filePath, FileMode.Create))
            //{
            //    await file.CopyToAsync(stream);
            //}

            //return Ok("/images/articles/" + fileName);
        }
    }
 }

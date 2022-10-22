using GeckosoftImages.Models;
using GeckosoftImages.Requests;
using GeckosoftImages.Services;
using GeckosoftImages.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GeckosoftImages.Controllers
{
    [Route("api/images")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage([FromForm] ImageRequest imageRequest)
        {
            var imageResponse = await _imageService.UploadImage(imageRequest);
            return Ok(imageResponse);
        }

        [HttpGet]
        public IActionResult GetImages()
        {
            throw new NotImplementedException();
        }

        [HttpPut]
        [Route("{name}")]
        public Task<IActionResult> ResizeImage(string name, int width, int height)
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        [Route("{name}")]
        public IActionResult DeleteImage(string name)
        {
            throw new NotImplementedException();
        }
    }
}

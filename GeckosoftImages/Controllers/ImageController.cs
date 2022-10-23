using GeckosoftImages.Models;
using GeckosoftImages.Requests;
using GeckosoftImages.Services;
using GeckosoftImages.Interfaces;
using Microsoft.AspNetCore.Mvc;
using GeckosoftImages.Responses;
using System.Net.Mime;
using GeckosoftImages.Exceptions;
using Microsoft.AspNetCore.Connections.Features;
using System.ComponentModel.DataAnnotations;

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

        /// <summary>
        /// Upload an image
        /// </summary>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(ImageResponse))]
        public async Task<IActionResult> UploadImage([FromForm] ImageRequest imageRequest)
        {
            var imageResponse = await _imageService.UploadImage(imageRequest);
            return Ok(imageResponse);
        }

        /// <summary>
        /// Get images file names as a list
        /// </summary>
        [HttpGet]
        public IActionResult GetImages()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Resize an image width and height
        /// </summary>
        [HttpPut]
        [Route("{name}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ImageResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ImageResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ImageResponse))]
        public async Task<IActionResult> ResizeImage(
            string name,
            [Range(0, int.MaxValue)] int width = 0,
            [Range(0, int.MaxValue)] int height = 0
            )
        {
            ImageResponse imageResponse;
            try
            {
                imageResponse = await _imageService.ResizeImage(name, width, height);
            }
            catch (FileNotFoundException e)
            {
                imageResponse = new ImageResponse { 
                    Success = false,
                    Status = StatusCodes.Status404NotFound,
                    Detail = e.Message,
                    FileName = name
                };
                return NotFound(imageResponse);
            }
            catch (TooManyFilesException e)
            {
                imageResponse = new ImageResponse {
                    Success = false,
                    Status = StatusCodes.Status400BadRequest,
                    Detail = e.Message,
                    FileName = name
                };
                return BadRequest(imageResponse);
            }
            return Ok(imageResponse);
        }

        /// <summary>
        /// Delete an image by name
        /// </summary>
        [HttpDelete]
        [Route("{name}")]
        public IActionResult DeleteImage(string name)
        {
            throw new NotImplementedException();
        }
    }
}

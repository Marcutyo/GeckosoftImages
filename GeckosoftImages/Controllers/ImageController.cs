using GeckosoftImages.Requests;
using GeckosoftImages.Interfaces;
using Microsoft.AspNetCore.Mvc;
using GeckosoftImages.Responses;
using GeckosoftImages.Exceptions;
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
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ValidationProblemDetails))]
        public async Task<IActionResult> UploadImage([FromForm] ImageRequest imageRequest)
        {
            var imageResponse = await _imageService.UploadImage(imageRequest);
            return Ok(imageResponse);
        }

        /// <summary>
        /// Get images ordered by name
        /// </summary>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ImagesEnumerableResponse))]
        public IActionResult GetImages()
        {
            var images = _imageService.GetImages();
            return Ok(images);
        }

        /// <summary>
        /// Resize an image width and height
        /// </summary>
        [HttpPut]
        [Route("{name}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ImageResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ValidationProblemDetails))]
        public async Task<IActionResult> ResizeImage(
            string name,
            [Range(0, int.MaxValue)] int width,
            [Range(0, int.MaxValue)] int height
            )
        {
            ImageResponse imageResponse;
            try
            {
                imageResponse = await _imageService.ResizeImage(name, width, height);
            }
            catch (FileNotFoundException e)
            {
                return NotFound(new ErrorResponse(e));
            }
            catch (TooManyFilesException e)
            {
                return BadRequest(new ErrorResponse(e));
            }
            return Ok(imageResponse);
        }

        /// <summary>
        /// Delete an image by name
        /// </summary>
        [HttpDelete]
        [Route("{name}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ImageResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
        public IActionResult DeleteImage(string name)
        {
            try
            {
                _imageService.DeleteImageByName(name);
            }
            catch (FileNotFoundException e)
            {
                return NotFound(new ErrorResponse(e));
            }
            catch (TooManyFilesException e)
            {
                return BadRequest(new ErrorResponse(e));
            }

            return Ok(new ImageResponse());
        }
    }
}

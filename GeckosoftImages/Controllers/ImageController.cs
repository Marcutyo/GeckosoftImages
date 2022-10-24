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
        private readonly IBackgroundQueue<ImageResizeRequest> _queue;

        public ImageController(
            IImageService imageService,
            IBackgroundQueue<ImageResizeRequest> queue)
        {
            _imageService = imageService;
            _queue = queue;
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
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ValidationProblemDetails))]
        public IActionResult ResizeImage(
            string name,
            [Range(1, int.MaxValue)] int width,
            [Range(1, int.MaxValue)] int height
            )
        {
            string imgPath;
            try
            {
                imgPath = _imageService.GetImagePathByName(name);;
            }
            catch (FileNotFoundException e)
            {
                return NotFound(new ErrorResponse(e));
            }
            catch (TooManyFilesException e)
            {
                return BadRequest(new ErrorResponse(e));
            }

            var imageResizeRequest = new ImageResizeRequest(imgPath, width, height);

            _queue.Enqueue(imageResizeRequest);
            return Accepted();
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

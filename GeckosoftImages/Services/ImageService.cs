using GeckosoftImages.Helpers;
using GeckosoftImages.Requests;
using GeckosoftImages.Responses;
using GeckosoftImages.Interfaces;

namespace GeckosoftImages.Services
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _environment;

        public ImageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<ImageResponse> UploadImage(ImageRequest imageRequest)
        {
            var uniqueFileName = FileHelper.GetUniqueFileName(imageRequest.Image.FileName);
            var uploads = Path.Combine(_environment.WebRootPath, "uploads");
            var filePath = Path.Combine(uploads, uniqueFileName);

            Directory.CreateDirectory(uploads);

            await imageRequest.Image.CopyToAsync(new FileStream(filePath, FileMode.Create));

            return new ImageResponse { Success = true, FilePath = filePath };
        }
    }
}

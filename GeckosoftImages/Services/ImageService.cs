using GeckosoftImages.Helpers;
using GeckosoftImages.Requests;
using GeckosoftImages.Responses;
using GeckosoftImages.Interfaces;
using System.Drawing;
using GeckosoftImages.Exceptions;
using System.Drawing.Imaging;

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

        public Task<ImageResponse> ResizeImage(string name, int width, int height)
        {
            var image = GetImageByName(name);
            var resizedImage = new Bitmap(image, new Size(width, height));

            using var imageStream = new MemoryStream();
            resizedImage.Save(imageStream, ImageFormat.Jpeg);
            var imageBytes = imageStream.ToArray();

            throw new NotImplementedException();
        }

        private Image GetImageByName(string name)
        {
            var uploads = Path.Combine(_environment.WebRootPath, "uploads");

            string[] imagePaths = Directory.GetFiles(uploads, searchPattern: name + ".*");
            if (imagePaths.Length > 1)
            {
                throw new TooManyFilesException();
            }
            string imagePath = imagePaths.First();

            var image = Image.FromStream(new FileStream(imagePath, FileMode.Open));
            return image;
        }
    }
}

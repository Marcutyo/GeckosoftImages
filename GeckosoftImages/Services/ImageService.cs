using GeckosoftImages.Helpers;
using GeckosoftImages.Requests;
using GeckosoftImages.Responses;
using GeckosoftImages.Interfaces;
using GeckosoftImages.Exceptions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

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

            return new ImageResponse { Success = true, FilePath = filePath, FileName = uniqueFileName};
        }

        public async Task<ImageResponse> ResizeImage(string name, int width, int height)
        {
            string imgPath = GetImagePathByName(name);

            string imgFileName = Path.GetFileNameWithoutExtension(imgPath);
            string imgFileExt = Path.GetExtension(imgPath);
            string? directoryName = Path.GetDirectoryName(imgPath);
            string tmpImg = Path.Combine(directoryName, "temp" + imgFileExt);

            using (var imgStream = File.OpenRead(imgPath))
            {
                var image = await Image.LoadAsync(imgStream);
                image.Mutate(x => x.Resize(width, height));
                await image.SaveAsync(tmpImg);
            }
            
            File.Move(tmpImg, imgPath, overwrite: true);

            return new ImageResponse { Success = true, FileName = imgFileName, FilePath = imgPath };
        }

        private string GetImagePathByName(string name)
        {
            var uploads = Path.Combine(_environment.WebRootPath, "uploads");

            string[] imgPaths = Directory.GetFiles(uploads, searchPattern: name + ".*");
            if (imgPaths.Length == 0)
                throw new FileNotFoundException($"Cannot find existing file with name '{name}'");
            if (imgPaths.Length > 1)
            {
                throw new TooManyFilesException($"More than 1 file with name '{name}' exists. Cannot handle this request");
            }
            string imgPath = imgPaths.First();

            return imgPath;
        }
    }
}

using GeckosoftImages.Exceptions;
using GeckosoftImages.Helpers;
using GeckosoftImages.Interfaces;
using GeckosoftImages.Models;
using GeckosoftImages.Requests;
using GeckosoftImages.Responses;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace GeckosoftImages.Services
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _uploadsPath;

        public ImageService(IWebHostEnvironment environment)
        {
            _environment = environment;

            _uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
        }

        public ImagesEnumerableResponse GetImages()
        {
            IEnumerable<string> imgPaths = Directory.EnumerateFiles(_uploadsPath);

            var imgsFileName = from imgPath in imgPaths
                               orderby imgPath
                               select new ImageFileModel(
                                   Path.GetFileNameWithoutExtension(imgPath)
                                   )
                               ;

            return new ImagesEnumerableResponse { Success = true, Data = imgsFileName };
        }

        public async Task<ImageResponse> UploadImage(ImageRequest imageRequest)
        {
            var uniqueFileName = FileHelper.GetUniqueFileName(imageRequest.Image.FileName);
            var uploads = Path.Combine(_environment.WebRootPath, "uploads");
            var filePath = Path.Combine(uploads, uniqueFileName);

            Directory.CreateDirectory(uploads);

            using var imgFileStream = new FileStream(filePath, FileMode.Create);
            await imageRequest.Image.CopyToAsync(imgFileStream);

            var imageModel = new ImageFileModel(uniqueFileName);

            return new ImageResponse { Success = true, Data = imageModel };
        }

        public void DeleteImageByName(string name)
        {
            string imgPath = GetImagePathByName(name);
            File.Delete(imgPath);
        }

        public async Task<ImageResponse> ResizeImage(ImageResizeRequest request)
        {
            string imgPath = request.ImagePath;

            string imgFileName = Path.GetFileNameWithoutExtension(imgPath);
            string imgFileExt = Path.GetExtension(imgPath);
            string? directoryName = Path.GetDirectoryName(imgPath);
            string tmpImg = Path.Combine(directoryName, "temp" + imgFileExt);

            using (var imgStream = File.OpenRead(imgPath))
            {
                var image = await Image.LoadAsync(imgStream);
                image.Mutate(x => x.Resize(request.Width, request.Height));

                await image.SaveAsync(tmpImg);
            }

            File.Move(tmpImg, imgPath, overwrite: true);

            var imageModel = new ImageFileModel(imgFileName);

            return new ImageResponse { Success = true, Data = imageModel };
        }

        public string GetImagePathByName(string name)
        {
            string[] imgPaths = Directory.GetFiles(_uploadsPath, searchPattern: name + ".*");
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

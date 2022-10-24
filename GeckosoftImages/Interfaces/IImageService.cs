using GeckosoftImages.Requests;
using GeckosoftImages.Responses;

namespace GeckosoftImages.Interfaces
{
    public interface IImageService
    {
        Task<ImageResponse> UploadImage(ImageRequest imageRequest);
        Task<ImageResponse> ResizeImage(ImageResizeRequest imageResizeRequest);
        ImagesEnumerableResponse GetImages();
        void DeleteImageByName(string name);
        string GetImagePathByName(string name);
    }
}

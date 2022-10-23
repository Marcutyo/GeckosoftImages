using GeckosoftImages.Requests;
using GeckosoftImages.Responses;

namespace GeckosoftImages.Interfaces
{
    public interface IImageService
    {
        Task<ImageResponse> UploadImage(ImageRequest imageRequest);
        Task<ImageResponse> ResizeImage(string name, int width, int height);
        ImagesEnumerableResponse GetImages();
        void DeleteImageByName(string name);
    }
}

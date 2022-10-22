using GeckosoftImages.Requests;
using GeckosoftImages.Responses;

namespace GeckosoftImages.Interfaces
{
    public interface IImageService
    {
        Task<ImageResponse> UploadImage(ImageRequest imageRequest);
    }
}

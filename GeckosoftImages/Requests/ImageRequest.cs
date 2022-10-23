using System.ComponentModel.DataAnnotations;

namespace GeckosoftImages.Requests
{
    public class ImageRequest
    {
        [Required]
        public IFormFile Image { get; set; }

        public ImageRequest(IFormFile image)
        {
            Image = image;
        }
    }
}

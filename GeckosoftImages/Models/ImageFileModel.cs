using System.ComponentModel.DataAnnotations;

namespace GeckosoftImages.Models
{
    public class ImageFileModel
    {
        [Required]
        public string FileName { get; set; }

        public ImageFileModel(string fileName)
        {
            FileName = fileName;
        }
    }
}

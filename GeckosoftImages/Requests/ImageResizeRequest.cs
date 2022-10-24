namespace GeckosoftImages.Requests
{
    public class ImageResizeRequest
    {
        public string ImagePath { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public ImageResizeRequest(string fileName, int width, int height)
        {
            ImagePath = fileName;
            Width = width;
            Height = height;
        }
    }
}

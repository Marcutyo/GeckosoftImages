namespace GeckosoftImages.Responses
{
    public class ErrorResponse
    {
        public bool Success { get; set; } = false;
        public string Type { get; set; }
        public string Detail { get; set; }

        public ErrorResponse(Exception e)
        {
            Type = e.Source;
            Detail = e.Message;
        }
    }
}

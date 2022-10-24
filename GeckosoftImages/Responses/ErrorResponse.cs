namespace GeckosoftImages.Responses
{
    public class ErrorResponse
    {
        public bool Success { get; set; } = false;
        public string Detail { get; set; }

        public ErrorResponse(Exception e)
        {
            Detail = e.Message;
        }
    }
}

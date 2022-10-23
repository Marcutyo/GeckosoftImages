using System.Runtime.Serialization;

namespace GeckosoftImages.Exceptions
{
    public class TooManyFilesException : Exception
    {
        public TooManyFilesException()
        {
        }

        public TooManyFilesException(string? message) : base(message)
        {
        }

        public TooManyFilesException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}

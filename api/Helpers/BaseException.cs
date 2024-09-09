using System.Net;

namespace api.Helpers
{
    public class BaseException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public BaseException(HttpStatusCode statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
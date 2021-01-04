using System.Net;

namespace Crabot.Rest
{
    public class OperationResult<T>
    {
        public OperationResult(T data, HttpStatusCode statusCode, 
            string reasonPhrase, string error)
        {
            Data = data;
            StatusCode = statusCode;
            ReasonPhrase = reasonPhrase;
            Error = error;
        }

        public T Data { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string ReasonPhrase { get; set; }
        public string Error { get; set; }
    }
}

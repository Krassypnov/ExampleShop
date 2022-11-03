using Microsoft.AspNetCore.Http;

namespace Order.Core.Message
{
    public class ServiceResult
    {
        public string? Message { get; set; }
        public int StatusCode { get; set; }

        public ServiceResult(string message, int statusCode)
        {
            Message = message;
            StatusCode = statusCode;
        }
    }
}

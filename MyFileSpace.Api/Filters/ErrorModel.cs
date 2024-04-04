namespace MyFileSpace.Api.Filters
{
    internal class ErrorModel
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public string? Details { get; set; }

        public ErrorModel(int statusCode, string? message, string? details = null)
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
        }

        public ErrorModel(int statusCode, Exception exception)
        {
            StatusCode = statusCode;
            Message = exception.Message;
            Details = exception.StackTrace;
        }
    }
}

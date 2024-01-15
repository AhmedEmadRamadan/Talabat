namespace Talabat.APIs.Errors
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string? ErrorMessage { get; set; }
        public ApiResponse(int statuscode, string? message = null)
        {
            StatusCode = statuscode;
            ErrorMessage = message ?? GetDefaultMessage(StatusCode);
        }

        private static string? GetDefaultMessage(int statusCode)
        {
            return statusCode switch
            {
                400 => "Bad Request",
                401 => "You Aren't Authorized",
                404 => "Resource Not Found",
                500 => "Internal Server Error",
                _ => null
            };
        }
    }
}

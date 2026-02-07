namespace HospitalBooking.Application.Common.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }

    public ApiResponse() { }

    public ApiResponse(T data, string message = "Success")
    {
        Success = true;
        Message = message;
        Data = data;
    }

    public ApiResponse(string message)
    {
        Success = true;
        Message = message;
    }
}

public class ApiResponse : ApiResponse<object>
{
    public ApiResponse()
    {
    }

    public ApiResponse(string message) : base(message)
    {
    }
    
    // For error responses, typically used by Middleware
    public static ApiResponse<T> Failure<T>(List<string> errors, string message = "Validation Failed")
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }

    public static ApiResponse Failure(string message)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            Errors = new List<string> { message }
        };
    }
}

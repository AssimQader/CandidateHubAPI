namespace CandidateHub.API.Models
{
    public class ApiResponse<T>
    {
        public string Status { get; set; } = "success";
        public string? Message { get; set; }
        public T? Data { get; set; }
        public List<ValidationError>? Errors { get; set; }

        public static ApiResponse<T> Success(T? data = default, string? message = null)
        {
            return new ApiResponse<T> { Data = data, Message = message, Status = "success" };
        }

        public static ApiResponse<T> Failed(string message, List<ValidationError>? errors = null)
        {
            return new ApiResponse<T> { Status = "error", Message = message, Errors = errors };
        }
    }

    public class ValidationError
    {
        public string Field { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}

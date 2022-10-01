namespace MovieAPI.Models
{
    public class ApiResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = String.Empty;
        public object Data { get; set; } = Array.Empty<string>() ;
    }
}

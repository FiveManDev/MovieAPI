namespace MovieAPI.Models
{
    public class ApiResponse
    {
        public object? StatusCode { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
    }
}

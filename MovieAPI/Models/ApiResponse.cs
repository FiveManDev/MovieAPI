﻿namespace MovieAPI.Models
{
    public class ApiResponse
    {
        public bool? IsSuccess { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
    }
}

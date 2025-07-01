namespace TodoApi.Models
{
    public class ErrorResponse
    {
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int Status { get; set; }
        public string Detail { get; set; } = string.Empty;
        public Dictionary<string, string[]>? Errors { get; set; }
        public string TraceId { get; set; } = string.Empty;
    }
} 
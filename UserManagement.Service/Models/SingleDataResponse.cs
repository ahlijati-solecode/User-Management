namespace User_Management.Models
{
    public class SingleDataResponse
    {
        public int code { get; set; }
        public string? status { get; set; }
        public string? message { get; set; }
        public object? data { get; set; }
    }
}
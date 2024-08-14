namespace User_Management.Models
{
    public class ListDataResponse
    {
        public int code { get; set; }
        public string? status { get; set; }
        public string? message { get; set; }
        public object? data { get; set; }
        public int total { get; set; }
    }
}
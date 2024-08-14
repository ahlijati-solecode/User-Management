namespace User_Management.Models.Requests
{
    public class LgActivityRequest
    {
        public int page { get; set; }

        public int size { get; set; }

        public string? sort { get; set; }

        public string? username { get; set; }

        public string? startDate { get; set; }

        public string? endDate { get; set; }
    }
}
namespace User_Management.Models.Responses.RoleAccess
{
    public class RoleAccessHistoryResponse
    {
        public string User { get; set; } = null!;
        public string Activity { get; set; } = null!;
        public string? Note { get; set; }
        public DateTime? Time { get; set; } = DateTime.Now;
        public int No { get; internal set; }
    }
}

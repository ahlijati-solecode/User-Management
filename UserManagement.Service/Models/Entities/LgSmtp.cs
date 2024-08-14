namespace User_Management.Models.Entities
{
    public partial class LgSmtp
    {
        public int Id { get; set; }
        public int? RefSmtpId { get; set; }
        public string? SmtpHost { get; set; }
        public string? SmtpUsername { get; set; }
        public string? SmtpPassword { get; set; }
        public int? SmtpPort { get; set; }
        public string? ApprovalStatus { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedTime { get; set; }
    }
}
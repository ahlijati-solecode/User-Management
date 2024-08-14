namespace User_Management.Models.Dtos.Role
{
    public class LgRolePagedDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int Id { get; set; }

        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public string ApprovalStatus { get; set; }
        public string Status { get; set; }
    }
}
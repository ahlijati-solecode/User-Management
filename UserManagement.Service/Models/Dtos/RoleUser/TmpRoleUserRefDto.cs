namespace User_Management.Models.Dtos.UserRole
{
    public class TmpRoleUserRefDto
    {
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string ParentId { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Departement { get; set; } = null!;
        public string Id { get; set; }
        public bool? IsApprover { get; set; }
    }
   
}

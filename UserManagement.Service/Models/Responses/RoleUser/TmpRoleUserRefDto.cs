namespace User_Management.Models.Responses.UserRole
{
    public class TmpRoleUserReResponse
    {
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string ParentId { get; set; } = null!;
        public string Departement { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Id { get; set; }
        public bool? IsApprover { get; set; }
    }
   
}

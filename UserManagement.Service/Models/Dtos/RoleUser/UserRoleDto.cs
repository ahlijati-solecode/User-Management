namespace User_Management.Models.Dtos.UserRole
{
    public class RoleUserDto
    {
        public int RoleId { get; set; }
        public string Username { get; set; }
        public int? Id { get; internal set; }
        public bool IsActive { get; set; }
        public bool? IsApprover { get; set; }
    }

}

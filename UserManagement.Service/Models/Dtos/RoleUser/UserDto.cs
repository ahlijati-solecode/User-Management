namespace User_Management.Models.Dtos.UserRole
{
    public class UserDto
    {
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string Username { get; set; } = null!;
        public int Id { get; set; }
        public bool? IsApprover { get; set; }
    }   
}

namespace User_Management.Models.Dtos.Role
{
    public class RoleDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public string Note { get; set; }

        public int Id { get; set; }
    }
}
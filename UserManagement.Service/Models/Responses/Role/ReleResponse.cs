namespace User_Management.Models.Responses.Role
{
    public class ReleResponse
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public int Id { get; set; }
    }
}
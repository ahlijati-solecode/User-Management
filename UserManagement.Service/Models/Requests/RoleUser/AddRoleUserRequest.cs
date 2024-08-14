namespace User_Management.Models.Requests.RoleUser
{
    public class AddRoleUserRequest
    {
        public int RoleId { get; set; }
        public string Username { get; set; }
        public bool IsApprover { get; set; }
    }
}

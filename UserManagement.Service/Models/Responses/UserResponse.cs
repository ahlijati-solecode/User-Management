namespace User_Management.Models.Responses.UserAccess
{
    public class UserResponse
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Departement { get; set; }
        public bool? IsApprover { get; set; }
    }
}

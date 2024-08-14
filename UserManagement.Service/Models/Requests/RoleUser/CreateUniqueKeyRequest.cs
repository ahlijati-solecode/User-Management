namespace User_Management.Models.Requests.RoleUser
{
    public class CreateUniqueKeyRequest
    {
        public int RoleUserId { get; set; }
        public bool JustView { get; set; } = false;
    }
}

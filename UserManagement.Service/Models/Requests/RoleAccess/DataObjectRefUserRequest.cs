namespace User_Management.Models.Requests.RefUserAccess
{
    public class DataObjectRefUserRequest
    {
        public virtual ICollection<EditRefUserAccessRequest> data { get; set; }
    }
}

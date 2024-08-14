namespace User_Management.Models.Requests.UserAccess
{
    public class BaseUserAccessRefRequest
    {
        public int MenuId { get; set; }
        public bool? IsView { get; set; }
        public bool? IsCreate { get; set; }
        public bool? IsEdit { get; set; }
        public bool? IsDelete { get; set; }
    }
}

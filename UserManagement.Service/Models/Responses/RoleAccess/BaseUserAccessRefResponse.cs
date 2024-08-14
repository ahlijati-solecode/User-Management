using User_Management.Models.Responses.UserAccess;

namespace User_Management.Models.Responses.RoleAccess
{
    public class BaseRoleAccessRefResponse
    {
        public MenuResponse Menu { get; set; }
        public bool? IsView { get; set; }
        public bool? IsCreate { get; set; }
        public bool? IsEdit { get; set; }
        public bool? IsDelete { get; set; }
    }
}

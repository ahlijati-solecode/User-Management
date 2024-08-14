using User_Management.Models.Requests.UserAccess;

namespace User_Management.Models.Responses.RoleAccess
{
    public class RoleAccessResponse
    {
        public int Id { get; set; }
        public int? RoleId { get; set; }
        public int Name { get; set; }
        public bool IsActive { get; set; }

        public IEnumerable<BaseUserAccessRefRequest> Children { get; set; }
    }
    public class UserAccessControlResponse: BaseRoleAccessRefResponse
    {

    }
}

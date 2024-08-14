using User_Management.Models.Responses.UserAccess;
using static Shared.Constants.ApiConstants;

namespace User_Management.Models.Responses.RoleAccess
{
    public class RoleAccessPagedResponse
    {
        public string? Name { get; set; }
        public int Id { get; set; }
        public bool IsActive { get; set; }

        public string Status
        {
            get
            {
                return IsActive ? StatusEnum.Active.ToString() : StatusEnum.InActive.ToString();
            }
        }
        public string? ApprovalStatus { get; set; }
        public IEnumerable<BaseRoleAccessRefResponse> Children { get; set; }
    }
}

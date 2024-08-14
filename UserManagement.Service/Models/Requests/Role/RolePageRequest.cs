using Shared.Models.Requests;
using System.ComponentModel;
using User_Management.Constants;
using static Shared.Constants.ApiConstants;

namespace User_Management.Models.Requests.Role
{
    public class RolePageRequest : BasePagedRequest
    {
        [DefaultValue("")]
        public string? Name { get; set; }

        [DefaultValue(RoleConstants.StatusEnum.Active)]
        public RoleConstants.StatusEnum? Status { get; set; }

        public ActivityEnum? ApprovalStatus { get; set; }

        [DefaultValue("id asc")]
        public string Sort { get; set; } = "id asc";
    }
}
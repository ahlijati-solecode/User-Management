using Shared.Models.Requests;
using System.ComponentModel;
using User_Management.Constants;

namespace User_Management.Models.Requests.RefUserAccess
{
    public class RefUserAccessPageRequest : BasePagedRequest
    {
        [DefaultValue("")]
        public string? Name { get; set; }
        [DefaultValue(UserAccessConstant.StatusEnumRefUser.Active)]
        public UserAccessConstant.StatusEnumRefUser? Status { get; set; }
        public UserAccessConstant.ActivityEnumRefUser? ApprovalStatus { get; set; }
        [DefaultValue("idrole asc")]
        public string? Sort { get; set; }
    }
}

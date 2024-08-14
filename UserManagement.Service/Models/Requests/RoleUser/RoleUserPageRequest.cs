using Shared.Models.Requests;
using System.ComponentModel;
using static Shared.Constants.ApiConstants;

namespace User_Management.Models.Requests.RoleUser
{
    public class RoleUserPageRequest : BasePagedRequest
    {
        [DefaultValue("")]
        public string? Name { get; set; }

        public int? Status { get; set; }

        public ActivityEnum? ApprovalStatus { get; set; }
    }
}

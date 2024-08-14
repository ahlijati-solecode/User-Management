using Shared.Models.Dtos.Core;
using User_Management.Models.Dtos.Role;

namespace User_Management.Models.Dtos.User
{
    public partial class LgRoleAccessDto : AuditEntityWithApproveDto
    {
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
        public string Activity { get; set; } = null!;
        public string Note { get; set; } = null!;
        public int? ParentId { get; set; }

        public virtual RoleDto Role { get; set; } = null!;
    }
}

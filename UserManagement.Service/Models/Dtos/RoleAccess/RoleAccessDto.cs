using Shared.Models.Dtos.Core;
using User_Management.Models.Dtos.Role;

namespace User_Management.Models.Dtos.User
{
    public class RoleAccessDto : AuditEntityWithApproveDto
    {
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
        public string? Name { get; set; }
        public string? ApprovalStatus { get; set; }
        public virtual RoleDto Role { get; set; } = null!;
        public virtual ICollection<MdRoleAccessRefDto> MdUserAccessRefs { get; set; }
        public virtual ICollection<LgRoleAccessDto> LgUserAccesses { get; set; }
        public virtual ICollection<LgRoleAccessRefDto> LgUserAccessRefs { get; set; }

    }
}

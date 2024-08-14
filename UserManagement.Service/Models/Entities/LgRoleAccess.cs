using Shared.Models.Entities;

namespace User_Management.Models.Entities
{
    public partial class LgRoleAccess :AuditEntityWithApprove
    {
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
        public string Activity { get; set; } = null!;
        public string? Note { get; set; } = null!;
        public int? ParentId { get; set; }

        public virtual MdRoleAccess? Parent { get; set; }
        public virtual MdRole Role { get; set; } = null!;
    }
}

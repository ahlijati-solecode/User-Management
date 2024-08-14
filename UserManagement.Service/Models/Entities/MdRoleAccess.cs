using Shared.Models.Entities;

namespace User_Management.Models.Entities
{
    public partial class MdRoleAccess : AuditEntityWithApprove
    {
        public MdRoleAccess()
        {
            LgUserAccessRefs = new HashSet<LgRoleAccessRef>();
            LgUserAccesses = new HashSet<LgRoleAccess>();
            MdUserAccessRefs = new HashSet<MdRoleAccessRef>();
        }

        public int RoleId { get; set; }
        public bool IsActive { get; set; }
       
        public virtual MdRole Role { get; set; } = null!;
        public virtual ICollection<LgRoleAccessRef> LgUserAccessRefs { get; set; }
        public virtual ICollection<LgRoleAccess> LgUserAccesses { get; set; }
        public virtual ICollection<MdRoleAccessRef> MdUserAccessRefs { get; set; }
    }
}

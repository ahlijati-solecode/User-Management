using Shared.Models.Entities;

namespace User_Management.Models.Entities
{
    public partial class MdRoleAccessRef : AuditEntity
    {
        public MdRoleAccessRef()
        {
            LgUserAccessRefs = new HashSet<LgRoleAccessRef>();
        }

        public int RefMenuId { get; set; }
        public int RefUserAccess { get; set; }
        public bool? IsView { get; set; }
        public bool? IsCreate { get; set; }
        public bool? IsEdit { get; set; }
        public bool? IsDelete { get; set; }
        

        public virtual MdRefMenu RefMenu { get; set; } = null!;
        public virtual MdRoleAccess RefUserAccessNavigation { get; set; } = null!;
        public virtual ICollection<LgRoleAccessRef> LgUserAccessRefs { get; set; }
    }
}

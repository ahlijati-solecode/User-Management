using Shared.Models.Entities;

namespace User_Management.Models.Entities
{
    public partial class MdRole : AuditEntity
    {
        public MdRole()
        {
            LgRoleHistories = new HashSet<LgRole>();
        }

        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }

        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedTime { get; set; }
        public virtual ICollection<LgRole> LgRoleHistories { get; set; }
        public virtual ICollection<LgRoleAccess> LgUserAccesses { get; set; }
        public virtual ICollection<MdRoleAccess> MdUserAccesses { get; set; }
        public bool? ApprovalStatus { get; set; }
        public virtual ICollection<MdRoleUser> MdRoleUsers { get; set; }
        public virtual ICollection<LgRoleUser> LgRoleUsers { get; set; }
        public virtual ICollection<TmpRoleUser> TmpRoleUsers { get; set; }
    }
}
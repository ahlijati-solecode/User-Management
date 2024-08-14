using Shared.Models.Entities;
using System;
using System.Collections.Generic;

namespace User_Management.Models.Entities
{
    public partial class MdRoleUser : AuditEntityWithApprove
    {
        public MdRoleUser()
        {
            LgRoleUsers = new HashSet<LgRoleUser>();
            MdRoleUserRefs = new HashSet<MdRoleUserRef>();
            TmpRoleUsers = new HashSet<TmpRoleUser>();
        }

        public int RoleId { get; set; }
        public bool IsActive { get; set; }

        public virtual MdRole Role { get; set; } = null!;
        public virtual ICollection<LgRoleUser> LgRoleUsers { get; set; }
        public virtual ICollection<MdRoleUserRef> MdRoleUserRefs { get; set; }
        public virtual ICollection<TmpRoleUser> TmpRoleUsers { get; set; }
    }
}

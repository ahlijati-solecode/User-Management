using Shared.Models.Entities;
using System;
using System.Collections.Generic;

namespace User_Management.Models.Entities
{
    public partial class TmpRoleUser : BaseAuditEntity, IEntityKey<string>
    {
        public TmpRoleUser()
        {
            TmpRoleUserRefs = new HashSet<TmpRoleUserRef>();
        }

        public string Id { get; set; } = null!;
        public int? RoleId { get; set; }
        public int? ParentId { get; set; }
        public bool IsActive { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }

        public virtual MdRoleUser? Parent { get; set; }
        public virtual MdRole? Role { get; set; }
        public virtual ICollection<TmpRoleUserRef> TmpRoleUserRefs { get; set; }
    }
}

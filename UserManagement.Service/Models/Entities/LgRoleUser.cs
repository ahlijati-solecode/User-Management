using Shared.Models.Entities;
using System;
using System.Collections.Generic;

namespace User_Management.Models.Entities
{
    public partial class LgRoleUser : AuditEntityWithApprove
    {
        public LgRoleUser()
        {
            LgRoleUserRefs = new HashSet<LgRoleUserRef>();
        }

        public int RoleId { get; set; }
        public int ParentId { get; set; }
        public bool IsActive { get; set; }
        public string? Note { get; set; }
        private string _activity;

        public string Activity
        {
            get { return _activity; }
            set
            {
                _activity = value;
                if (value == "Submitted")
                {
                    DeletedBy = null;
                    DeletedDate = null;
                }
            }
        }


        public virtual MdRoleUser Parent { get; set; } = null!;
        public virtual MdRole Role { get; set; } = null!;
        public virtual ICollection<LgRoleUserRef> LgRoleUserRefs { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace User_Management.Models.Entities
{
    public partial class TmpRoleUserRef
    {
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string ParentId { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Id { get; set; }
        public string? Departement { get; set; }

        public virtual TmpRoleUser Parent { get; set; } = null!;
        public bool? IsApprover { get;  set; }
    }
}

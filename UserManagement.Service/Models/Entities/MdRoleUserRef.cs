using System;
using System.Collections.Generic;

namespace User_Management.Models.Entities
{
    public partial class MdRoleUserRef
    {
        public int ParentId { get; set; }
        public string Username { get; set; } = null!;
        public string? Email { get; set; }
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Departement { get; set; }
        public bool? IsApprover { get; set; }
        public virtual MdRoleUser Parent { get; set; } = null!;
    }
}

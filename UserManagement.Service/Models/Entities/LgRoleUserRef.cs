using System;
using System.Collections.Generic;

namespace User_Management.Models.Entities
{
    public partial class LgRoleUserRef
    {
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public int ParentId { get; set; }
        public string Username { get; set; } = null!;
        public string? Departement { get; set; }
        public int Id { get; set; }
        public bool? IsApprover { get; set; }
        public virtual LgRoleUser Parent { get; set; } = null!;
        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}

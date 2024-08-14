using Shared.Models.Entities;

namespace User_Management.Models.Entities
{
    public partial class LgRole: AuditEntity
    {
        public string User { get; set; } = null!;
        public string Activity { get; set; } = null!;
        public string? Note { get; set; }
        public DateTime? Time { get; set; }
        public int RoleId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsAdmin { get; set; }
   

        public virtual MdRole Role { get; set; } = null!;
    }
}
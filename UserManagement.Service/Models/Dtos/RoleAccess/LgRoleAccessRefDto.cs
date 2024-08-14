using Shared.Models.Dtos.Core;

namespace User_Management.Models.Dtos.User
{
    public partial class LgRoleAccessRefDto : AuditEntityDto
    {
        public int RefMenuId { get; set; }
        public int RefUserAccess { get; set; }
        public bool? IsView { get; set; }
        public bool? IsCreate { get; set; }
        public bool? IsEdit { get; set; }
        public bool? IsDelete { get; set; }
        public int? ParentId { get; set; }
    }
}

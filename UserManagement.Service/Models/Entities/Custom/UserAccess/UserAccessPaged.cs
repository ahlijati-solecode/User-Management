using Shared.Models.Dtos.Core;

namespace User_Management.Models.Entities.Custom.RefUserAccess
{
    public partial class MdUserAccessPaged : MdRoleAccess, IFieldStatus
    {
        public string? ApprovalStatus { get; set; }
        public string? IsActive { get; set; }
        
        public string Name { get; set; }
    }
}

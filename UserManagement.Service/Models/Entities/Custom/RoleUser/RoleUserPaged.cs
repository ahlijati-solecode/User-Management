using Shared.Models.Core;

namespace User_Management.Models.Entities.Custom.RoleUser
{
    public class RoleUserPaged
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string ApprovalStatus { get; set; }
        public int LogId { get; set; }
        public List<User> Users { get; set; }
    }
}

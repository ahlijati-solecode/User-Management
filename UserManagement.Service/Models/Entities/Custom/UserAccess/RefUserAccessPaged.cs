using static User_Management.Constants.RoleAccessConstant;

namespace User_Management.Models.Entities.Custom.RefUserAccess
{
    public class RefUserAccessPaged
    {
        public RefUserAccessPaged()
        {
        }
        public int? Id { get; set; }
        public int? RoleId { get; set; }
        public string RoleName { get; set; }
        public virtual ICollection<MdRefMenu> LgRefMenu { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public string ApprovalStatus { get; set; }
        public string Status
        {
            get
            {
                return string.Empty;
            }
        }
        public string ApvStatus
        {
            get
            {
                return ApprovalStatus;
            }

        }
    }
}

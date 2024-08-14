using static Shared.Constants.ApiConstants;

namespace User_Management.Models.Dtos.UserRole
{
    public class LgRoleAccessDto
    {
        public int Id { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public string ApprovalStatus { get; set; }
        public string Status
        {
            get
            {
                return IsActive ? StatusEnum.Active.ToString() : StatusEnum.InActive.ToString();
            }
        }
        public List<UserDto> Users { get; set; }
    }

}

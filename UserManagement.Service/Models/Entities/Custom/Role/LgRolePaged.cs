using static User_Management.Constants.RoleConstants;

namespace User_Management.Models.Entities.Custom.Role
{
    public class LgRolePaged
    {
        public LgRolePaged()
        {
        }

        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int Id { get; set; }

        public bool Is_Active { get; set; }
        public bool Is_Admin { get; set; }
        public string ApprovalStatus { get; set; }

        public string Status
        {
            get
            {
                if (Is_Active)
                    return StatusEnum.Active.ToString();
                return StatusEnum.InActive.ToString();
            }
        }
    }
}
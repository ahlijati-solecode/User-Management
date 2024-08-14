namespace User_Management.Models.Dtos.User
{
    public class LgRefUserAccessPagedDto
    {
        public LgRefUserAccessPagedDto()
        {
        }
        public int? Id { get; set; }
        public int? RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public string ApprovalStatus { get; set; }

    }
}

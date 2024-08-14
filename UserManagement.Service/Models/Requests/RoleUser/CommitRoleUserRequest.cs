using System.ComponentModel.DataAnnotations;

namespace User_Management.Models.Requests.RoleUser
{
    public class CommitRoleUserRequest
    {
        public int? RoleUserId { get; set; }
        public bool IsActive { get; set; }
        public int RoleId { get; set; }
    }

    public class EditRoleUserRequest
    {
        [Required]
        public int RoleUserId { get; set; }
        public bool IsActive { get; set; }
        public int RoleId { get; set; }
    }
}

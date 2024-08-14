using System.ComponentModel.DataAnnotations;

namespace User_Management.Models.Requests.UserAccess
{
    public class BaseUserAccessRequest
    {
        [Required]
        public int RoleId { get; set; }
        public bool IsActive { get; set; }

        public List<BaseUserAccessRefRequest> Children { get; set; }


    }
}

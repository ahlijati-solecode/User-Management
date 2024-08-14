using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace User_Management.Models.Requests.Role
{
    public class EditeRoleRequest : BaseRoleRequest
    {
        [Required(ErrorMessage = "The Id is required")]
        public int Id { get; set; }

        [DefaultValue("")]
        public string? Note { get; set; }
    }
}
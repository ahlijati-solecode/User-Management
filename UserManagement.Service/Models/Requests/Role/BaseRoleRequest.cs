using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace User_Management.Models.Requests.Role
{
    public class BaseRoleRequest
    {
        [Required(ErrorMessage = "The Name is required")]
        [DefaultValue("")]
        public string Name { get; set; } = null!;

        [DefaultValue("")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "The IsActive is required")]
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "The IsAdmin is required")]
        public bool IsAdmin { get; set; }
    }
}
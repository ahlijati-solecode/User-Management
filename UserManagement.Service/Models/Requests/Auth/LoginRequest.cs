using System.ComponentModel.DataAnnotations;

namespace User_Management.Models.Requests.Auth
{
    public class LoginRequest
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public bool IsKeepLogged { get; set; } = false;
    }
}
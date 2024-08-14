using System.ComponentModel;

namespace User_Management.Models.Requests.Role
{
    public class RejectedRequest
    {
        [DefaultValue("")]
        public string? Note { get; set; }
    }
}
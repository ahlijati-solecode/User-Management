using System.ComponentModel;

namespace User_Management.Models.Requests.RefUserAccess
{
    public class RejectedRefUserAccessRequest
    {
        [DefaultValue("")]
        public string? Note { get; set; }
    }
}

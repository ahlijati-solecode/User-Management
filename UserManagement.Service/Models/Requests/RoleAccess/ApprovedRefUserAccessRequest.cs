using System.ComponentModel;

namespace User_Management.Models.Requests.RefUserAccess
{
    public class ApprovedRefUserAccessRequest
    {
        [DefaultValue("")]
        public string? Note { get; set; }

    }
}

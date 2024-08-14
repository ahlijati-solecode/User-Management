using Shared.Models.Filters;

namespace User_Management.Models.Filters
{
    public class RoleUserFilter : IFilterStatus
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public string ApprovalStatus { get; set; }

        public override string ToString()
        {
            return $"Name : {Name}, Status : {Status} , ApprovalStatus : {ApprovalStatus}";
        }
    }
}
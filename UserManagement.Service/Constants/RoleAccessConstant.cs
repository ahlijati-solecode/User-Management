namespace User_Management.Constants
{
    public class RoleAccessConstant
    {

        public const string TableName = "md_role_access";
        public const string TableLogName = "lg_role_access";
        public const string ReferenceField = "parent_id";

        public const string NeedProccedByApproval = "The variable needs to be approved or rejected before it can be edited";
        public const string CraetedMessage = "Created Role Access {0}";
        public const string GetPagedMessage = "Get Paged Role Access {0}";
        public const string UpdatedMessage = "Updated Role Access {0}";
        public const string ApprovedMessage = "Approved Role Access {0}";
        public const string StatusApprovalMessage = "Can't modify data, because data has been {0}";
        public const string RejectedMessage = "Rejected Role Access {0}";
        public const string DeletedMessage = "Deleted Role Access ({0}) {1}";
        public const string DataNotFound = "Role Access {0} not found ";
        public const string GenerateDataMessage = "Generated Data Success";
        public const string GenerateDataFailMessage = "Generated Data Failed";
        public const string NotExitsMessage = "Role Access with ID {0} not found";


        public enum AddStateEnum
        {
            Success,
            RoleNotFound,
            MenuNotFound,
            DuplicateRole
        }
        public enum UpdateStateEnum
        {
            Success,
            RoleNotFound,
            MenuNotFound,
            UserAccessNotFound,
            WaitingApprovedByApproval,
            DuplicateRole

        }
        public enum DeleteStateEnum
        {
            Success,
            UserAccessNotFound,
            WaitingApprovedByApproval
        }

        public enum GetStateEnum
        {
            Success,
            UserAccessNotFound
        }
        public enum ApprovedStateEnum
        {
            Success,
            UserAccessNotFound,
            HasBeenProcced
        }

        public enum RejectedStateEnum
        {
            Success,
            UserAccessNotFound,
            HasBeenProcced
        }

        public enum GetRoleAccessHistoiresEnum
        {
            Success,
            RoleAccessNotFound,
        }
    }
}

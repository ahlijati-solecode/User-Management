namespace User_Management.Constants
{
    public class RoleConstants
    {
        public const string TableName = "md_role";
        public const string TableLogName = "lg_role";
        public const string ReferenceField = "role_id"; 
        public const string CraetedMessage = "Created Role {0}";
        public const string GetPagedMessage = "Get Paged Role {0}";
        public const string UpdatedMessage = "Updated Role ({0}) {1}";
        public const string StatusApprovalMessage = "Can't modify data, because data has been {0}";
        public const string ApprovalMessage = "{1} Role {0}";
        public const string DeletedMessage = "Deleted Role ({0}) {1}";
        public const string NotExitsMessage = "Role with ID {0} not found";
        public const string ConnectedToAccessControlListMessage = "its connected to Access Control List";
        public const string ConnectedToUserRoleMessage = "its connected to User Role";
        public const string ConnectedToUserRoleAndAccessControlListMessage = "its connected to Access Control List & User Role";
        public enum StatusEnum
        {
            InActive,
            Active
        }

        public enum ApprovalStateEnum
        {
            Success,
            RoleNotFound,
            HasProcessed
        }

        public enum DeleteStateEnum
        {
            Success,
            RoleNotFound,
            WaitingApprovedByApproval,
            NotFound,
            UsedInRoleAccess,
            UsedInRoleUser,
            UsedInRoleAccessAndUserRole
        }
        public enum AddStateEnum
        {
            Success,
            DuplicateRoleName
        }
        public enum UpdateStateEnum
        {
            Success,
            RoleNotFound,
            NotFound,
            DuplicateRoleName,
            WaitingApprovedByApproval,
        }

        public enum GetRoleHistoiresEnum
        {
            Success,
            RoleNotFound,
        }

        public enum GetByIdStateEnum
        {
            Success,
            RoleNotFound,
        }
    }
}
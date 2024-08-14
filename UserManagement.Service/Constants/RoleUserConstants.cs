namespace User_Management.Constants
{
    public class RoleUserConstants
    {
        public const string TableName = "md_role_user";
        public const string TableLogName = "lg_role_user";
        public const string ReferenceField = "parent_id";

        public const string NotExitsMessage = "Data with ID {0} not found";
        public const string RoleNotExitsMessage = "Role with ID {0} not found";
        public const string ParamNotExitsMessage = "Param with ID {0} not found";
        public const string ParentNotFoundMessage = "Parent Not Found";
        public enum CreateUniqueStateEnum
        {
            Success,
            RoleUserNotFound,
            WaitingApprovedByApproval
        }
        public enum AddRoleUserTemporaryStatEnum
        {
            Success,
            ParentNotFound,
            RoleNotFound,
            UserNotFound,
            DuplicateRole,
            DuplicateUser
        }
        public enum UpdateRoleUserTemporaryStatEnum
        {
            Success,
            ParentNotFound,
            RoleNotFound,
            ItemNotFound,
            UserNotFound,
            DuplicateUser
        }
        public enum DeletedRoleUserTemporaryStatEnum
        {
            Success,
            ParentNotFound,
            ItemNotFound,
        }

        public enum SaveStatEnum
        {
            Success,
            ParentNotFound,
            ItemNotFound
        }
        public enum DeleteStateEnum
        {
            Success,
            ItemNotFound,
            WaitingApprovedByApproval
        }

        public enum ApprovedStateEnum
        {
            Success,
            ItemNotFound,
            HasBeenProcessed
        }
        public enum GetPagedUserRoleHistoryStateEnum
        {
            Success,
            ItemNotFound,
        }

        public enum GetByIdUserRoleHistoryStateEnum
        {
            Success,
            ItemNotFound,
        }
    }
}
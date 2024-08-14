namespace User_Management.Constants
{
    public class RefMenuConstant
    {
        public const string TableName = "md_ref_user_access";
        public const string TableLogName = "lg_ref_user_access_hist";
        public const string ReferenceField = "idrefuseraccess";

        public const string CreatedMessage = "Created Menu {0}";
        public const string GetPagedMessage = "Get Menu {0}";
        public const string UpdatedMessage = "Updated Menu ({0}) {1}";
        public const string ApprovedMessage = "Approved Menu ({0}) {1}";
        public const string StatusApprovalMessage = "Can't modify data, because data has been {0}";
        public const string RejectedMessage = "Rejected Menu ({0}) {1}";
        public const string DeletedMessage = "Deleted Menu ({0}) {1}";
        public const string NotExitsMessage = "Menu with ID {0} not found";
        public const string FailedToGenerated = "Failed To Generate Data For User Access";

        public enum GenerateStatusEnum
        {
            CanDuplicate,
            FiledToGenerated,
            Success
        }

        public enum UpdateStatusEnum
        {
            Duplicate,
            FiledToGenerated,
            Success
        }

        public enum ApprovedStatusEnum
        {
            HasBeenProcessed,
            Success
        }
        public enum DeletedStatusEnum
        {
            RoleNotFound,
            Success
        }
        public enum RejectedStatusEnum
        {
            HasBeenProcessed,
            Success
        }
    }
}

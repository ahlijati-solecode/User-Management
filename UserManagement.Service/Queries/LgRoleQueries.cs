using User_Management.Constants;
using static Shared.Constants.ApiConstants;

namespace User_Management.Queries
{
    public static class LgRoleQueries
    {
        public static string CheckStatusApproval = $" SELECT  distinct  activity FROM {RoleConstants.TableLogName}  where  role_id=@role_id limit 1";

        public static string ApprovalStatusQuery = $"(select activity from {RoleConstants.TableLogName} where  AND  role_id = role.id order by id desc  limit 1 ) ";

        public const string SelectPagedQuery = " t.id,log.name,log.description ,log.is_admin,log.is_active";

        public static string PagedQuery = $"SELECT @select FROM {RoleConstants.TableName}  role  @filter ORDER BY @sort limit @limit offset @offset";
        public static string Query = $"SELECT @select FROM {RoleConstants.TableName}  role  @filter";

        public const string JoinQuery = $" join {RoleConstants.TableLogName} log on log.id = (select {RoleConstants.TableLogName}.id from {RoleConstants.TableLogName}  where  role_id = t.id order by id desc  limit 1 ) ";
    }
}
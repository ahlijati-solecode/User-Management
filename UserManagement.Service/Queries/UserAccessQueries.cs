using User_Management.Constants;

namespace User_Management.Queries
{
    public class UserAccessQueries
    {
        public const string SelectPagedQuery = " t.id,role.name,log.is_active as isActive";
        public const string JoinQuery = $" join md_role role ON role.id = ((select role_ref_id from {RoleAccessConstant.TableLogName} where parent_id = t.id order by id desc  limit 1 ) )  join lg_role_access log on log.id = ((select id from lg_role_access where parent_id = t.id order by id desc  limit 1 ) )   ";

    }
    public class RoleUserQueries
    {
        public const string SelectPagedQuery = " log.id as logId, t.id, role.name, log.is_active  as \"isActive\" ";
        public const string JoinQuery = $" join {RoleUserConstants.TableLogName} log on log.id = (select {RoleUserConstants.TableLogName}.id from {RoleUserConstants.TableLogName}  where  {RoleUserConstants.ReferenceField} = t.id order by id desc  limit 1 ) join md_role role on role.id = t.role_id"; 

    }
}

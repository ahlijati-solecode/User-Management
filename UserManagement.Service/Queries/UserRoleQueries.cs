namespace User_Management.Queries
{
    public class UserRoleQueries
    {
        public static string DeleteTemporaryRef = "Delete from tmp_role_user_ref where parent_id in (select id from tmp_role_user where created_date < '{0}')";
        public static string DeleteTemporary = "delete from tmp_role_user where created_date < '{0}'";
    }
}

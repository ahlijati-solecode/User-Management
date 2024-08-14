namespace User_Management.Queries
{
    public class LgRefMenuQueries
    {
        public static string SelectPagedQuery = string.Format(@" id,name");
        public static string Query = string.Format(@"SELECT @select FROM md_ref_menu  refmenu  @sort");
    }
}

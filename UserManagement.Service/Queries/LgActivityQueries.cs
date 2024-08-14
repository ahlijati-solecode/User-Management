namespace User_Management.Queries
{
    public static class LgActivityQueries
    {
        public const string getList = @"
                select username, activity, time 
                from lg_activity
            ";

        public const string countList = @"
                select count(*) as ""total""
                from lg_activity
            ";

        public const string insert = @"
                insert into lg_activity(username, activity, time)
                values(@username, @activity, cast(@time as timestamp))
            ";
    }
}

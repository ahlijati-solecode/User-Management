namespace User_Management.Constants
{
    public class UserConstants
    {
        
        public const string InvalidUserNameMessage = "Invalid Username and Password";
        public const string LoginSuccessMessage = "Login successful";
        public const string ActivityDirectoryNotConfigureMessage = "Active directory not Configure";

        public enum LoginStateEnum
        {
            Success,
            InvalidUsername,
            ActiveDirectoryNotConfigure
        }
        public enum SearchStateEnum
        {
            Success,
            ActiveDirectoryNotConfigure
        }
        public enum AccessCotntrolListState
        {
            Success,
            UserNotFound
        }
    }
}
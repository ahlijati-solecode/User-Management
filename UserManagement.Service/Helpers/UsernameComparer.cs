using Shared.Models.Core;

namespace User_Management.Helpers
{
    public class UsernameComparer : IEqualityComparer<User>
    {
        #region IEqualityComparer<Contact> Members

        public bool Equals(User x, User y)
        {
            return x.UserName.Equals(y.UserName);
        }

        public int GetHashCode(User obj)
        {
            return obj.UserName.GetHashCode();
        }

        #endregion
    }

}

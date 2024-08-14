using Novell.Directory.Ldap;
using Shared.Constants;
using Shared.Infrastructures.Repositories;
using Shared.Models.Core;
using Shared.Models.Entities.Custom.Configuration;

namespace User_Management.Infrastructures.Repositories
{
    public class LdapUserRepository : IUserRepository, IAuthenticationRepository, ISearchUserRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<LdapUserRepository> _logger;

        public LdapUserRepository(IHttpContextAccessor httpContextAccessor,
            ILogger<LdapUserRepository> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<User?> GetById(string userId)
        {
            var users = await SearchBy(userId);
            return users.FirstOrDefault();
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            try
            {
                if (_httpContextAccessor.HttpContext == null) return null;
                var user = CreateFakeUser();
                username = username.ToLower();
                user.UserName = username;
                var configuration = _httpContextAccessor.HttpContext.Items[ApiConstants.ActivityDirectoryConfiguration] as ActiveDirectory;
                if (configuration == null) throw new ArgumentNullException("configuration is null");
                LdapConnection ldapConnection = new LdapConnection();
                ldapConnection.Connect(configuration.Host.Replace("ldap://", String.Empty), configuration.Port);
                ldapConnection.Bind($"{configuration.Attribute ?? "uid"}={username},{configuration.BaseDN}", password);
                var ldapSearch = ldapConnection.Search(configuration.BaseDN, LdapConnection.ScopeSub, string.IsNullOrEmpty(username) ? "objectClass=inetOrgPerson" : $"(uid=*{username ?? "."}*)", new string[] { "cn", "mail", "departmentNumber" }, false);
                while (ldapSearch.HasMore())
                {
                    var nexEntry = ldapSearch.Next();
                    var attributeSet = nexEntry.GetAttributeSet();
                    var ineu = attributeSet.GetEnumerator();
                    while (ineu.MoveNext())
                    {
                        var attribute = (LdapAttribute)ineu.Current;
                        var name = attribute.Name;
                        switch (name)
                        {
                            case "cn":
                                user.FullName = attribute.StringValue;
                                break;

                            case "mail":
                                user.Email = attribute.StringValue;
                                break;

                            case "departmentNumber":
                                user.Departement = attribute.StringValue;
                                break;

                            default:
                                break;
                        }
                    }
                }
                return user;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message, exception);
                return null;
            }
        }
        public Task<List<User>> SearchBy(string userId)
        {
            return SearchBy(userId, false);
        }

        public Task<List<User>> SearchBy(string userId, bool isExact)
        {
            var list = new List<User>();
            var configuration = _httpContextAccessor.HttpContext.Items[ApiConstants.ActivityDirectoryConfiguration] as ActiveDirectory;
            if (configuration == null) throw new ArgumentNullException("configuration is null");
            LdapConnection ldapConnection = new LdapConnection();
            ldapConnection.ConnectionTimeout = 500;
            ldapConnection.Connect(configuration.Host.Replace("ldap://", String.Empty).Trim(), configuration.Port);
            ldapConnection.Bind($"{configuration.Attribute ?? "uid"}={configuration.UserDN},{configuration.BaseDN}", configuration.Password);
            var ldapSearch = ldapConnection.Search(configuration.BaseDN, LdapConnection.ScopeSub, string.IsNullOrEmpty(userId) ? "objectClass=inetOrgPerson" : (isExact ? $"(uid={userId ?? "."})" : $"(uid=*{userId ?? "."}*)"),
                new string[] { "cn", "uid", "sn", "mail", "departmentNumber" }, false);
            while (ldapSearch.HasMore())
            {
                try
                {
                    var nexEntry = ldapSearch.Next();
                    var attributeSet = nexEntry.GetAttributeSet();
                    var ineu = attributeSet.GetEnumerator();
                    var user = new User();
                    while (ineu.MoveNext())
                    {

                        var attribute = (LdapAttribute)ineu.Current;
                        var name = attribute.Name;
                        switch (name)
                        {
                            case "cn":
                                user.FullName = attribute.StringValue;
                                break;
                            case "mail":
                                user.Email = attribute.StringValue;
                                break;
                            case "uid":
                                user.UserName = attribute.StringValue.ToLower(); ;
                                break;
                            case "departmentNumber":
                                user.Departement = attribute.StringValue;
                                break;
                            default:
                                break;
                        }
                    }
                    list.Add(user);
                }
                catch (Exception)
                {
                    break;
                }

            }
            return Task.FromResult(list);
        }

        private User CreateFakeUser()
        {
            var user = new User()
            {
                Email = "dewangga.respati@gmail.com",
                UserName = "drespati",
                FullName = "Dewangga Respati",
                Authorizations = new List<string> { "authorization1", "authorization2" },
                Roles = new List<string> { "administrator" }
            };
            return user;
        }
    }
}

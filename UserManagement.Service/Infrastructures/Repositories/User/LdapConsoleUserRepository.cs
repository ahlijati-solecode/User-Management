using Novell.Directory.Ldap;
using Shared.Constants;
using Shared.Infrastructures.Repositories;
using Shared.Models.Core;
using Shared.Models.Entities.Custom.Configuration;
using System.Diagnostics;

namespace User_Management.Infrastructures.Repositories
{
    public class LdapConsoleUserRepository : IUserRepository, IAuthenticationRepository, ISearchUserRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<LdapUserRepository> _logger;

        public LdapConsoleUserRepository(IHttpContextAccessor httpContextAccessor,
            ILogger<LdapUserRepository> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<User?> GetById(string userId)
        {
            var users = await SearchBy(userId,true);
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
            var attributeSearching = isExact ? $"{userId}" : $"*{userId ?? "."}*";
            ProcessStartInfo processStartinfo = new ProcessStartInfo("/usr/bin/ldapsearch", $"-x -b \"{configuration.BaseDN}\" -H {configuration.Host} -D \"{configuration.Attribute}={configuration.UserDN},{configuration.BaseDN}\" -w {configuration.Password} \"({configuration.Attribute}={attributeSearching})\"")
            {
                CreateNoWindow = false,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            Process process = Process.Start(processStartinfo);
            string result = process.StandardOutput.ReadToEnd();
            var lines = result.Split(new[] { '\n' });
            var user = new User();
            foreach (var item in lines)
            {
                if (item.StartsWith("dn:"))
                {
                    user = new User();
                    list.Add(user);
                }
                else if (item.StartsWith("cn:"))
                {
                    user.FullName = item.Replace("cn:", string.Empty).Trim();
                }
                else if (item.StartsWith("mail:"))
                {
                    user.Email = item.Replace("mail:", string.Empty).Trim();
                }
                else if (item.StartsWith($"{configuration.Attribute}:"))
                {
                    user.UserName = item.Replace($"{configuration.Attribute}:", string.Empty).Trim();
                }
                else if (item.StartsWith("departmentNumber:"))
                {
                    user.Departement = item.Replace("departmentNumber:", string.Empty).Trim();
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

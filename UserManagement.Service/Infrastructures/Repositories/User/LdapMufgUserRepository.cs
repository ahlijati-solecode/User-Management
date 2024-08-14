using Novell.Directory.Ldap;
using Shared.Constants;
using Shared.Infrastructures.Repositories;
using Shared.Models.Core;
using Shared.Models.Entities.Custom.Configuration;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace User_Management.Infrastructures.Repositories
{
    public class LdapMufgUserRepository : IUserRepository, IAuthenticationRepository, ISearchUserRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<LdapMufgUserRepository> _logger;
        private const string defaultSearching = "(&(objectClass=person)(!(objectClass=computer)))";
        public LdapMufgUserRepository(IHttpContextAccessor httpContextAccessor,
            ILogger<LdapMufgUserRepository> logger)
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
                username = username.ToLower();
                if (_httpContextAccessor.HttpContext == null) return null;
                var user = new User();
                user.UserName = username;
                var configuration = _httpContextAccessor.HttpContext.Items[ApiConstants.ActivityDirectoryConfiguration] as ActiveDirectory;
                if (configuration == null) throw new ArgumentNullException("configuration is null");
                LdapConnection ldapConnection = new LdapConnection();
                var host = configuration.Host.Replace("ldap://", String.Empty);
                var domainName = GetDomainName(host);
                _logger.LogInformation($"Prepare Connection to AD host :{host} , port : {configuration.Port}");
                ldapConnection.Connect(host, configuration.Port);
                var dn = $"{username}@{domainName}";
                _logger.LogInformation($"Can connect to {host}");
                _logger.LogInformation($"Prepare Bind : {dn} | password : {password}");
                ldapConnection.Bind(dn, password);
                _logger.LogInformation($"Success Bind to {dn}");
                var filter = $"({configuration.Attribute ?? "uid"}={username})";
                _logger.LogInformation($"Prepare {filter}");
                try
                {
                    LdapSearchConstraints cons = ldapConnection.SearchConstraints;
                    cons.ReferralFollowing = true;
                    ldapConnection.Constraints = cons;
                    var ldapSearch = ldapConnection.Search(configuration.BaseDN, LdapConnection.ScopeSub, filter, new string[] { "cn", "mail", "department" }, false);
                    _logger.LogInformation($"Done Search data");

                    while (ldapSearch.HasMore())
                    {
                        _logger.LogInformation($"ldapSearch.Next()");
                        var nexEntry = ldapSearch.Next();
                        _logger.LogInformation($"nexEntry.GetAttributeSet()");
                        var attributeSet = nexEntry.GetAttributeSet();
                        _logger.LogInformation($"attributeSet.GetEnumerator()");
                        var ineu = attributeSet.GetEnumerator();
                        var loop = true;
                        while (loop)
                        {
                            try
                            {
                                ineu.MoveNext();

                                var attribute = (LdapAttribute)ineu.Current;
                                _logger.LogInformation($"(LdapAttribute)ineu.Current");
                                var name = attribute.Name;
                                _logger.LogInformation($"{DateTime.Now} attribute name {name}");
                                switch (name)
                                {
                                    case "cn":
                                        user.FullName = attribute.StringValue;
                                        _logger.LogInformation($"{DateTime.Now} attribute {name} setted");
                                        break;

                                    case "mail":
                                        user.Email = attribute.StringValue;
                                        _logger.LogInformation($"{DateTime.Now} attribute {name} setted");
                                        break;

                                    case "department":
                                        user.Departement = attribute.StringValue;
                                        _logger.LogInformation($"{DateTime.Now} attribute {name} setted");
                                        break;
                                    
                                    default:
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                loop = false;
                                _logger.LogError(ex, "Error while loop");
                                _logger.LogInformation($"{DateTime.Now} Current User : {JsonConvert.SerializeObject(user)}");
                                return user;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Un Handle exceptions");
                }
                _logger.LogInformation($"{DateTime.Now} Current User : {JsonConvert.SerializeObject(user)}");
                return user;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message, exception);
                return null;
            }
        }

        private string GetDomainName(string host)
        {
            var arrayHost = host.Split(new[] { '.' });
            return arrayHost[arrayHost.Length - 2];
        }

        public Task<List<User>> SearchBy(string userId)
        {
            userId = userId.ToLower();
            var list = new List<User>();
            var configuration = _httpContextAccessor.HttpContext.Items[ApiConstants.ActivityDirectoryConfiguration] as ActiveDirectory;
            if (configuration == null) throw new ArgumentNullException("configuration is null");
            LdapConnection ldapConnection = new LdapConnection();
            ldapConnection.ConnectionTimeout = 500;
            var host = configuration.Host.Replace("ldap://", String.Empty).Trim();
            var domainName = GetDomainName(host);
            _logger.LogInformation($"Prepare Connection to AD host :{host} , port : {configuration.Port}");
            var dn = $"{configuration.UserDN}@{domainName}";
            var filter = string.IsNullOrEmpty(userId) ? defaultSearching : $"(&({configuration.Attribute}=*{userId}*)(&(objectClass=person)(!(objectClass=computer))))";
            _logger.LogInformation($"filter {filter}");
            ldapConnection.Connect(host, configuration.Port);
            _logger.LogInformation($"{DateTime.Now} Can connect to {host}");
            var time = DateTime.Now;
            _logger.LogInformation($"{DateTime.Now} Prepare Bind : {dn} | password : {configuration.Password}");
            ldapConnection.Bind(dn, configuration.Password);
            _logger.LogInformation($"{DateTime.Now} Bind Success");
            try
            {
                LdapSearchConstraints cons = ldapConnection.SearchConstraints;
                cons.ReferralFollowing = true;
                ldapConnection.Constraints = cons;
                var attributeUser = configuration.Attribute.ToString();
                _logger.LogInformation($"{DateTime.Now} Search {configuration.BaseDN} filter {filter}");
                var ldapSearch = ldapConnection.Search(configuration.BaseDN, LdapConnection.ScopeSub, filter,
                new string[] { "cn", attributeUser, "mail", "department" }, false);
                _logger.LogInformation($"{DateTime.Now} Search Success");
                // var index = 0;
                while (ldapSearch.HasMore())
                {
                    var user = new User();

                    try
                    {
                        // _logger.LogInformation($"ldapSearch.Next() {ldapSearch.Count}");
                        _logger.LogInformation("========================== START =======================");
                        var task = Task.Run(() => ldapSearch.Next());
                        if (task.Wait(TimeSpan.FromMilliseconds(500)))
                        {
                            var nexEntry = task.Result;

                            _logger.LogInformation($"nexEntry.GetAttributeSet()");
                            var attributeSet = nexEntry.GetAttributeSet();
                            _logger.LogInformation($"attributeSet.GetEnumerator()");
                            var ineu = attributeSet.GetEnumerator();

                            var loop = true;
                            var cnSet = false;
                            var mailSet = false;
                            var departmentSet = false;
                            var usernameSet = false;
                            while (loop && (!cnSet || !mailSet || !departmentSet || !usernameSet))
                            {
                                try
                                {
                                    ineu.MoveNext();
                                    var attribute = (LdapAttribute)ineu.Current;
                                    var name = attribute.Name;
                                    _logger.LogInformation($"{DateTime.Now} attribute name {name}");
                                    switch (name)
                                    {
                                        case "cn":
                                            user.FullName = attribute.StringValue;
                                            _logger.LogInformation($"{DateTime.Now} attribute {name} setted {user.FullName}");
                                            cnSet = true;
                                            break;

                                        case "mail":
                                            user.Email = attribute.StringValue;
                                            _logger.LogInformation($"{DateTime.Now} attribute {name} setted {user.Email}");
                                            mailSet = true;
                                            break;

                                        case "department":
                                            user.Departement = attribute.StringValue;
                                            _logger.LogInformation($"{DateTime.Now} attribute {name} setted {user.Departement}");
                                            departmentSet = true;
                                            break;

                                        default:
                                            break;
                                    }
                                    if (attributeUser == name)
                                    {
                                        user.UserName = attribute.StringValue.ToLower();
                                        _logger.LogInformation($"{DateTime.Now} attribute {name} setted {user.UserName}");
                                        usernameSet = true;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    loop = false;
                                    _logger.LogError(ex, "Error while loop attribute");
                                    break;
                                }

                            }
                            _logger.LogInformation("========================== END =========================");

                            list.Add(user);
                        }
                        else
                        {
                            _logger.LogInformation($"{DateTime.Now} timeout when try to get next user");
                            throw new TimeoutException("timeout when try to get next user");
                        }
                        // index++;
                        
                        // if (ldapSearch.Count - 3 <= index)
                        //    break;

                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception.Message, exception);
                        _logger.LogInformation($"{DateTime.Now} error when try to get next user");
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(SearchBy)} Un handle Exception", ex);
            }

            _logger.LogInformation($"{DateTime.Now} list constructed. ready to return");

            return Task.FromResult(list);
        }


    }
}

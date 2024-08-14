using Shared.Constants;
using Shared.Infrastructures.Repositories;
using Shared.Models;
using Shared.Models.Core;
using Shared.Infrastructures.Repositories.Configuration;
using User_Management.Infrastructures.Repositories.UserAccess;
using static User_Management.Constants.UserConstants;
using User_Management.Infrastructures.Repositories.RoleUser;
using User_Management.Models.Dtos.User;
using AutoMapper;
using static Shared.Constants.ApiConstants;
using User_Management.Constants;

namespace User_Management.Infrastructures.Services
{
    public class UserService : IUserService
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly IRoleUserRefRepository _userRefRepository;
        private readonly IRoleAccessRefRepository _roleAccessRepository;
        private readonly IMapper _mapper;
        private readonly IActivityRepository _activityRepository;
        private readonly IUnitOfWorks _unitOfWorks;
        private readonly ISearchUserRepository _searchUserRepository;

        public UserService(IConfigurationRepository configurationRepository,
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor,
            IAuthenticationRepository authenticationRepository,
            IRoleUserRefRepository userRefRepository,
            IRoleAccessRefRepository roleAccessRefRepository,
            IMapper mapper,
            IActivityRepository activityRepository,
            IUnitOfWorks unitOfWorks,
            ISearchUserRepository searchUserRepository)
        {
            _configurationRepository = configurationRepository;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _authenticationRepository = authenticationRepository;
            _userRefRepository = userRefRepository;
            _roleAccessRepository = roleAccessRefRepository;
            _mapper = mapper;
            _activityRepository = activityRepository;
            _unitOfWorks = unitOfWorks;
            _searchUserRepository = searchUserRepository;
        }


        public async Task<Result<AccessCotntrolListState, List<MdRoleAccessRefDto>?>> GetAccessControl()
        {
            var userName = _httpContextAccessor.HttpContext?.CurrentUser()?.UserName;
            if (userName == null)
                return new(AccessCotntrolListState.UserNotFound, null);
            var userRoles = await _userRefRepository.GetByUserName(userName);
            var userMenu = await _roleAccessRepository.GetByRoles(userRoles.Select(n => n.Parent.Role.Id).ToArray());
            if (userRoles.Any(n => n.IsApprover == true))
            {
                userMenu.Add(new Models.Entities.MdRoleAccessRef()
                {
                    Id = -1,
                    RefMenu = new Models.Entities.MdRefMenu()
                    {
                        Id = -1,
                        Key = "TASK_LIST",
                        Name = "Task List"

                    },
                    IsCreate = true,
                    IsDelete = true,
                    IsEdit = true,
                    IsView = true
                });
            }
            return new(AccessCotntrolListState.Success, _mapper.Map<List<MdRoleAccessRefDto>>(userMenu.ToList()));
        }

        public async Task<Result<LoginStateEnum, User?>> LoginAsync(string username, string password)
        {
            try
            {
                var configuration = await _configurationRepository.GetActiveDirectory();
                if (configuration != null)
                {
                    if (_httpContextAccessor.HttpContext != null && !_httpContextAccessor.HttpContext.Items.ContainsKey(ApiConstants.ActivityDirectoryConfiguration))
                        _httpContextAccessor.HttpContext.Items.Add(ApiConstants.ActivityDirectoryConfiguration, configuration);
                    var user = await _authenticationRepository.LoginAsync(username, password);
                    if (user == null)
                    {
                        _ = await _activityRepository.AddAsync(UserConstants.InvalidUserNameMessage + $" ({username})");
                        return new(LoginStateEnum.InvalidUsername, null);
                    }
                    var userRoles = await _userRefRepository.GetByUserName(user.UserName);
                    user.Roles = userRoles.Select(n => n.Parent?.Role?.Name).Distinct().ToList();
                    _ = await _activityRepository.AddAsync(UserConstants.LoginSuccessMessage, user.UserName);
                    return new(LoginStateEnum.Success, user);
                }
                return new(LoginStateEnum.ActiveDirectoryNotConfigure, null);
            }
            finally
            {
                await _unitOfWorks.SaveChangesAsync();
            }
        }

        public async Task<Result<SearchStateEnum, List<User>>> Search(string key)
        {
            var configuration = await _configurationRepository.GetActiveDirectory();
            if (configuration != null)
            {
                if (_httpContextAccessor.HttpContext != null && !_httpContextAccessor.HttpContext.Items.ContainsKey(ApiConstants.ActivityDirectoryConfiguration))
                    _httpContextAccessor.HttpContext.Items.Add(ApiConstants.ActivityDirectoryConfiguration, configuration);
                return new(SearchStateEnum.Success, await _searchUserRepository.SearchBy(key));

            }
            return new(SearchStateEnum.ActiveDirectoryNotConfigure, null);
        }


    }
}
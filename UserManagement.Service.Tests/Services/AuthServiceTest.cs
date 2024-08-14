using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shared.Configurations;
using Shared.Infrastructures.Repositories;
using Shared.Models.Core;
using Shared.Service.Tests.Controllers;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Shared.Infrastructures.Repositories.Configuration;
using Shared.Models.Entities.Custom.Configuration;
using Xunit;
using User_Management.Infrastructures.Services;
using User_Management.Infrastructures.Repositories.UserAccess;
using User_Management.Models.Requests.Auth;
using User_Management.Models.Entities;
using static User_Management.Constants.UserConstants;
using User_Management.Infrastructures.Repositories.RoleUser;
using AutoMapper;
using User_Management.Constants;

namespace UserManagement.Service.Tests.Controllers
{
    public class AuthServiceTest : BaseControllerFactoryTest
    {
        private readonly UserService _service;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly AppSettings _appSettings;
        private readonly IRoleUserRefRepository _userRefRepository;
        private readonly IRoleAccessRefRepository _roleAccessRefRepository;
        private readonly ISearchUserRepository _searchUserRepository;
        private readonly IActivityRepository _activityRepository;
        private readonly IUnitOfWorks _unitOfWorks;
        public AuthServiceTest(WebApiTesterFactory factory) : base(factory)
        {
            _configurationRepository = Substitute.For<IConfigurationRepository>();
            _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            _userRepository = Substitute.For<IUserRepository>();
            _authenticationRepository = Substitute.For<IAuthenticationRepository>();
            _userRefRepository = Substitute.For<IRoleUserRefRepository>();
            _roleAccessRefRepository = Substitute.For<IRoleAccessRefRepository>();
            _appSettings = new AppSettings() { Secret = "TFuJQ3DUMjlr1qay" };
            var appOptions = Substitute.For<IOptions<AppSettings>>();
            IMapper _mapper = GenerateMapper();
            appOptions.Value.Returns(_appSettings);
            _activityRepository = Substitute.For<IActivityRepository>();
            _roleAccessRefRepository = Substitute.For<IRoleAccessRefRepository>();
            _searchUserRepository = Substitute.For<ISearchUserRepository>();
            _unitOfWorks = Substitute.For<IUnitOfWorks>();
            _service = new UserService(_configurationRepository, _userRepository, _httpContextAccessor,
                _authenticationRepository,
                _userRefRepository
                , _roleAccessRefRepository
                , _mapper
                , _activityRepository,
                _unitOfWorks,
                _searchUserRepository);
        }

        [Theory]
        [InlineData("/user/auth/login")]
        public async Task Post_Invalid_Request(string url)
        {
            // Arrange
            var client = _factory.CreateClient();
            var content = new StringContent("{}", Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync(url, content);

            // Assert
            var message = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("{\"code\":400,\"status\":\"BadRequest\",\"data\":\"The Password field is required.|The UserName field is required.\"}", message);
        }

        [Fact]
        public async Task Post_Login_Success()
        {
            // Arrange
            var loginRequest = new LoginRequest();
            loginRequest.UserName = "test";
            loginRequest.Password = "password";
            var activeDirectories = new ActiveDirectory()
            {
                Host = "localhost",
                Attribute = "uid",
                BaseDN = "dn=xxx",
                Port = 789
            };
            _configurationRepository.GetActiveDirectory().Returns(activeDirectories);
            var userRoles = new List<MdRoleUserRef>();
            userRoles.Add(new MdRoleUserRef()
            {
                Username = "test",
                Parent = new MdRoleUser()
                {
                    Role = new MdRole()
                    {
                        Id = 1
                    }
                }
            });

            _userRefRepository.GetByUserName(Arg.Any<string>()).Returns(userRoles);

            var roleAccess = new List<MdRoleAccessRef>();
            roleAccess.Add(new MdRoleAccessRef()
            {
                RefUserAccess = 1,
                IsCreate = true,
                IsDelete = true,
            });
            _roleAccessRefRepository.GetByRoles(Arg.Any<int[]>()).Returns(roleAccess);

            _authenticationRepository.LoginAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(CreateFakeUser());
            //Act
            var result = await _service.LoginAsync(loginRequest.UserName, loginRequest.Password);

            //Assert
            _ = await _configurationRepository.Received(1).GetActiveDirectory();
            _ = await _userRefRepository.Received(1).GetByUserName(Arg.Any<string>());
            _ = await _roleAccessRefRepository.Received(0).GetByRoles(Arg.Any<int[]>());
            _ = _httpContextAccessor.Received(3).HttpContext;
            _ = await _activityRepository.Received(1).AddAsync(Arg.Is<string>(n => n == UserConstants.LoginSuccessMessage), Arg.Any<string>());
            _ = await _authenticationRepository.Received(1).LoginAsync(Arg.Any<string>(), Arg.Any<string>());
            Assert.Equal(LoginStateEnum.Success, result.State);
        }

        [Fact]
        public async Task Post_Login_NotFound()
        {
            // Arrange
            var loginRequest = new LoginRequest();
            loginRequest.UserName = "test";
            loginRequest.Password = "password";
            var activeDirectories = new ActiveDirectory()
            {
                Host = "localhost",
                Attribute = "uid",
                BaseDN = "dn=xxx",
                Port = 789
            };
            _configurationRepository.GetActiveDirectory().Returns(activeDirectories);
            User user = null;
            _authenticationRepository.LoginAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(user);
            //Act
            var result = await _service.LoginAsync(loginRequest.UserName, loginRequest.Password);

            //Assert
            _ = await _configurationRepository.Received(1).GetActiveDirectory();
            _ = _httpContextAccessor.Received(3).HttpContext;
            _ = await _activityRepository.Received(1).AddAsync(Arg.Is<string>(n => n.Contains(UserConstants.InvalidUserNameMessage)), Arg.Any<string>());
            _ = await _authenticationRepository.Received(1).LoginAsync(Arg.Any<string>(), Arg.Any<string>());
            Assert.Equal(LoginStateEnum.InvalidUsername, result.State);
        }

        [Fact]
        public async Task Post_Login_Missing_Configuration()
        {
            // Arrange
            var loginRequest = new LoginRequest();
            loginRequest.UserName = "test";
            loginRequest.Password = "password";
            ActiveDirectory activeDirectories = null;

            _configurationRepository.GetActiveDirectory().Returns(activeDirectories);
            User user = null;
            _authenticationRepository.LoginAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(user);
            //Act
            var result = await _service.LoginAsync(loginRequest.UserName, loginRequest.Password);

            //Assert
            _ = await _configurationRepository.Received(1).GetActiveDirectory();
            _ = _httpContextAccessor.Received(0).HttpContext;
            _ = await _authenticationRepository.Received(0).LoginAsync(Arg.Any<string>(), Arg.Any<string>());
            Assert.Equal(LoginStateEnum.ActiveDirectoryNotConfigure, result.State);
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
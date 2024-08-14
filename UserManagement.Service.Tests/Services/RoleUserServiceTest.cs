using AutoMapper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using Shared.Infrastructures.Repositories;
using Shared.Infrastructures.Repositories.Configuration;
using Shared.Infrastructures.Repositories.TaskList;
using Shared.Infrastructures.Services.User;
using Shared.Models.Core;
using Shared.Models.Dtos.Core;
using Shared.Service.Tests.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using User_Management.Infrastructures.Repositories;
using User_Management.Infrastructures.Repositories.RoleUser;
using User_Management.Infrastructures.Repositories.UserAccess;
using User_Management.Infrastructures.Services;
using User_Management.Models.Dtos.User;
using User_Management.Models.Dtos.UserRole;
using User_Management.Models.Entities;
using User_Management.Models.Entities.Custom.RoleUser;
using User_Management.Models.Filters;
using Xunit;
using static Shared.Constants.ApiConstants;
using static User_Management.Constants.RoleAccessConstant;
using static User_Management.Constants.RoleUserConstants;

namespace UserManagement.Service.Tests.Controllers
{
    public class RoleUserServiceTest : BaseControllerFactoryTest
    {
        private readonly ILogRoleUserRepository _logRoleUserRepository;
        private readonly ILogRoleUserRefRepository _logRoleUserRefRepository;
        private readonly IRoleUserRefRepository _roleUserRefRepository;
        private readonly IRoleUserRepository _roleUserRepository;
        private readonly ITemporaryRoleUserRefRepository _temporaryRoleUserRefRepository;
        private readonly ITemporaryRoleUserRepository _temporaryRoleUserRepository;
        private readonly IUnitOfWorks _unitOfWorks;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly ISearchUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IActivityRepository _activityRepository;
        private readonly ITaskRepository _taskRepository;
        RoleUserService _service;

        public RoleUserServiceTest(WebApiTesterFactory factory) : base(factory)
        {
            _logRoleUserRepository = Substitute.For<ILogRoleUserRepository>();
            _logRoleUserRefRepository = Substitute.For<ILogRoleUserRefRepository>();
            _roleUserRefRepository = Substitute.For<IRoleUserRefRepository>();
            _roleUserRepository = Substitute.For<IRoleUserRepository>();
            _temporaryRoleUserRefRepository = Substitute.For<ITemporaryRoleUserRefRepository>();
            _temporaryRoleUserRepository = Substitute.For<ITemporaryRoleUserRepository>();
            _unitOfWorks = Substitute.For<IUnitOfWorks>();
            _roleRepository = Substitute.For<IRoleRepository>();
            _mapper = GenerateMapper();
            _userRepository = Substitute.For<ISearchUserRepository>();
            _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            _configurationRepository = Substitute.For<IConfigurationRepository>();
            _currentUserService = Substitute.For<ICurrentUserService>();
            _activityRepository = Substitute.For<IActivityRepository>();
            _taskRepository = Substitute.For<ITaskRepository>();
            _service = new RoleUserService(_logRoleUserRepository, _logRoleUserRefRepository,
                _roleUserRefRepository, _roleUserRepository, _temporaryRoleUserRefRepository,
                _temporaryRoleUserRepository, _unitOfWorks, _roleRepository, _mapper,
                _userRepository, _httpContextAccessor, _configurationRepository,
                _currentUserService, _activityRepository, _taskRepository);
        }

        [Fact]
        public async Task View_UserRole_History()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("User-Token", userToken);
            // Act
            var response = await client.GetAsync("user/user-roles?sort=id desc&page=1&Size=10");

            // Assert
            var message = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var data = JsonConvert.DeserializeObject(message) as JObject;
            Assert.NotNull(data);
            Assert.NotNull(data["code"]);
            Assert.NotNull(data["data"]);
            Assert.NotNull(data["data"]["items"] as JArray);
            var array = data["data"]["items"] as JArray;
            var userRoleId = 0;
            if (array.Any())
            {
                foreach (var item in array)
                {
                    var child = item as JObject;
                    Assert.NotNull(child["id"]);

                    userRoleId = Convert.ToInt32(child["id"]);
                    break;
                }
                response = await client.GetAsync($"user/user-roles/{userRoleId}/history?Page=1&Size=5&Sort=id%20asc");

                // Assert

                message = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                data = JsonConvert.DeserializeObject(message) as JObject;
                Assert.NotNull(data);
                Assert.NotNull(data["code"]);
                Assert.NotNull(data["data"]);
                Assert.NotNull(data["data"]["items"] as JArray);
                array = data["data"]["items"] as JArray;
                foreach (var item in array)
                {
                    var child = item as JObject;
                    Assert.NotNull(child["activity"]);
                }
            }
        }
        [Fact]
        public async Task Search_User_Role_By_Role_Name()
        {
            // Arrange
            var page = new PagedDto();
            var filter = new RoleUserFilter();
            filter.Name = "role";
            var rolePageds = new Paged<RoleUserPaged>()
            {
                Items = new[]
                {
                    new RoleUserPaged()
                    {
                        Id = 1,
                        Users= new List<User>
                        {
                            new User()
                            {
                                UserName ="xxx"
                            }
                        }
                    }
                }
            };
            _roleUserRepository.GetPaged(Arg.Any<RoleUserFilter>(), Arg.Any<int>(), Arg.Any<int>(),
          Arg.Any<string>()).Returns(rolePageds);
            var users = new List<LgRoleUserRef>();
            users.Add(new LgRoleUserRef());
            _logRoleUserRefRepository.GetUsers(Arg.Any<int>(), Arg.Any<int>()).Returns(users);
            //Act
            var result = await _service.GetPaged(page, filter);

            //Assert

            await _logRoleUserRefRepository.Received(1).GetUsers(Arg.Any<int>(), Arg.Any<int>());

            await _roleUserRepository.Received(1).GetPaged(Arg.Is<RoleUserFilter>(n =>
         !string.IsNullOrEmpty(filter.Name) &&
         n.Status == null &&
         n.ApprovalStatus == null), Arg.Is<int>(n => n == 1), Arg.Is<int>(n => n == 10),
          Arg.Is<string>(n => n == "id asc"));

        }

        [Fact]
        public async Task Search_User_Role_By_Status()
        {
            // Arrange
            var page = new PagedDto();
            var filter = new RoleUserFilter();
            filter.Status = "1";
            var rolePageds = new Paged<RoleUserPaged>()
            {
                Items = new[]
                {
                    new RoleUserPaged()
                    {
                        Id = 1,
                        Users= new List<User>
                        {
                            new User()
                            {
                                UserName ="xxx"
                            }
                        }
                    }
                }
            };
            _roleUserRepository.GetPaged(Arg.Any<RoleUserFilter>(), Arg.Any<int>(), Arg.Any<int>(),
          Arg.Any<string>()).Returns(rolePageds);
            var users = new List<LgRoleUserRef>();
            users.Add(new LgRoleUserRef());
            _logRoleUserRefRepository.GetUsers(Arg.Any<int>(), Arg.Any<int>()).Returns(users);
            //Act
            var result = await _service.GetPaged(page, filter);

            //Assert

            await _logRoleUserRefRepository.Received(1).GetUsers(Arg.Any<int>(), Arg.Any<int>());

            await _roleUserRepository.Received(1).GetPaged(Arg.Is<RoleUserFilter>(n =>
         string.IsNullOrEmpty(filter.Name) &&
         n.Status != null &&
         n.ApprovalStatus == null), Arg.Is<int>(n => n == 1), Arg.Is<int>(n => n == 10),
          Arg.Is<string>(n => n == "id asc"));

        }

        [Fact]
        public async Task Search_User_Role_By_Approval_Status()
        {
            // Arrange
            var page = new PagedDto();
            var filter = new RoleUserFilter();
            filter.ApprovalStatus = "Approved";
            var rolePageds = new Paged<RoleUserPaged>()
            {
                Items = new[]
                {
                    new RoleUserPaged()
                    {
                        Id = 1,
                        Users= new List<User>
                        {
                            new User()
                            {
                                UserName ="xxx"
                            }
                        }
                    }
                }
            };
            _roleUserRepository.GetPaged(Arg.Any<RoleUserFilter>(), Arg.Any<int>(), Arg.Any<int>(),
          Arg.Any<string>()).Returns(rolePageds);
            var users = new List<LgRoleUserRef>();
            users.Add(new LgRoleUserRef());
            _logRoleUserRefRepository.GetUsers(Arg.Any<int>(), Arg.Any<int>()).Returns(users);
            //Act
            var result = await _service.GetPaged(page, filter);

            //Assert

            await _logRoleUserRefRepository.Received(1).GetUsers(Arg.Any<int>(), Arg.Any<int>());

            await _roleUserRepository.Received(1).GetPaged(Arg.Is<RoleUserFilter>(n =>
         string.IsNullOrEmpty(filter.Name) &&
         n.Status == null &&
         n.ApprovalStatus == "Approved"), Arg.Is<int>(n => n == 1), Arg.Is<int>(n => n == 10),
          Arg.Is<string>(n => n == "id asc"));

        }

        [Fact]
        public async Task Search_User_Role_By_All()
        {
            // Arrange
            var page = new PagedDto();
            var filter = new RoleUserFilter();
            filter.ApprovalStatus = "Approved";
            filter.Name = "role";
            filter.Status = "1";
            var rolePageds = new Paged<RoleUserPaged>()
            {
                Items = new[]
                {
                    new RoleUserPaged()
                    {
                        Id = 1,
                        Users= new List<User>
                        {
                            new User()
                            {
                                UserName ="xxx"
                            }
                        }
                    }
                }
            };
            _roleUserRepository.GetPaged(Arg.Any<RoleUserFilter>(), Arg.Any<int>(), Arg.Any<int>(),
          Arg.Any<string>()).Returns(rolePageds);
            var users = new List<LgRoleUserRef>();
            users.Add(new LgRoleUserRef());
            _logRoleUserRefRepository.GetUsers(Arg.Any<int>(), Arg.Any<int>()).Returns(users);
            //Act
            var result = await _service.GetPaged(page, filter);

            //Assert

            await _logRoleUserRefRepository.Received(1).GetUsers(Arg.Any<int>(), Arg.Any<int>());

            await _roleUserRepository.Received(1).GetPaged(Arg.Is<RoleUserFilter>(n =>
         !string.IsNullOrEmpty(filter.Name) &&
         n.Status == "1" &&
         n.ApprovalStatus == "Approved"), Arg.Is<int>(n => n == 1), Arg.Is<int>(n => n == 10),
          Arg.Is<string>(n => n == "id asc"));

        }

        [Fact]
        public async Task Create_Role_User_Success()
        {
            // Arrange
            var uniqueId = Guid.NewGuid().ToString();
            var roleUser = new RoleUserDto();
            var latestLog = new LgRoleAccessRef();
            var tmpRoleUser = new TmpRoleUser()
            {
                TmpRoleUserRefs = new List<TmpRoleUserRef>()
                {
                    new TmpRoleUserRef()
                    {
                        Id="testId",
                        Email="test@gmail.com",
                        FullName="test testing",
                        Username ="test"
                    }
                }
            };
            var role = new MdRole();
            _roleRepository.GetById(Arg.Any<int>()).Returns(role);
            _temporaryRoleUserRepository.GetById(Arg.Any<string>()).Returns(tmpRoleUser);

            //Act
            var result = await _service.CommitRoleUserAsync(uniqueId, roleUser);

            //Assert
            await _temporaryRoleUserRepository.Received(0).AddAsync(Arg.Any<TmpRoleUser>());
            await _roleUserRefRepository.Received(1).AddAsync(Arg.Any<MdRoleUserRef>());
            await _roleUserRepository.Received(1).AddAsync(Arg.Any<MdRoleUser>());
            await _roleUserRepository.Received(0).GetById(Arg.Any<int>());
            await _roleUserRepository.Received(0).UpdateAsync(Arg.Any<MdRoleUser>());
            await _logRoleUserRefRepository.Received(1).AddAsync(Arg.Any<LgRoleUserRef>());
            await _logRoleUserRepository.Received(1).AddAsync(Arg.Any<LgRoleUser>());
            await _unitOfWorks.Received(2).SaveChangesAsync();
            await _activityRepository.Received(1).AddAsync(Arg.Any<string>());
            await _taskRepository.Received(0).AddAsync(Arg.Any<string>());
            Assert.Equal(SaveStatEnum.Success, result.State);
        }

        [Fact]
        public async Task Remove_User_From_Role()
        {
            // Arrange
            var uniqueId = Guid.NewGuid().ToString();
            var id = Guid.NewGuid().ToString();
            var roleRef = new TmpRoleUserRef()
            {
                Id = "testId",
                Email = "test@gmail.com",
                FullName = "test testing",
                Username = "test"
            };
            var tmpRoleUser = new TmpRoleUser()
            {
                TmpRoleUserRefs = new List<TmpRoleUserRef>()
                {
                    roleRef
                }
            };
            var role = new MdRole();
            _roleRepository.GetById(Arg.Any<int>()).Returns(role);
            _temporaryRoleUserRepository.GetById(Arg.Any<string>()).Returns(tmpRoleUser);
            _temporaryRoleUserRefRepository.GetById(Arg.Any<string>()).Returns(roleRef);
            //Act
            var result = await _service.DeleteRoleUserAsync(uniqueId, id);

            //Assert
            await _temporaryRoleUserRepository.Received(0).AddAsync(Arg.Any<TmpRoleUser>());
            await _roleUserRefRepository.Received(0).AddAsync(Arg.Any<MdRoleUserRef>());
            await _roleUserRepository.Received(0).AddAsync(Arg.Any<MdRoleUser>());
            await _roleUserRepository.Received(0).GetById(Arg.Any<int>());
            await _roleUserRepository.Received(0).UpdateAsync(Arg.Any<MdRoleUser>());
            await _logRoleUserRefRepository.Received(0).AddAsync(Arg.Any<LgRoleUserRef>());
            await _logRoleUserRepository.Received(0).AddAsync(Arg.Any<LgRoleUser>());
            await _unitOfWorks.Received(1).SaveChangesAsync();
            await _activityRepository.Received(0).AddAsync(Arg.Any<string>());
            await _taskRepository.Received(0).AddAsync(Arg.Any<string>());

            await _temporaryRoleUserRefRepository.Received(1).DeleteAsync(Arg.Any<TmpRoleUserRef>(), true);

            Assert.Equal(DeletedRoleUserTemporaryStatEnum.Success, result.State);
        }

        [Fact]
        public async Task Post_Update_Success()
        {
            // Arrange
            var uniqueId = Guid.NewGuid().ToString();
            var roleUser = new RoleUserDto();
            var latestLog = new LgRoleAccessRef();
            var tmpRoleUser = new TmpRoleUser();
            var role = new MdRole();
            _roleRepository.GetById(Arg.Any<int>()).Returns(role);
            _temporaryRoleUserRepository.GetById(Arg.Any<string>()).Returns(tmpRoleUser);
            //Act
            var result = await _service.CommitRoleUserAsync(uniqueId, roleUser);

            //Assert
            await _temporaryRoleUserRepository.Received(0).AddAsync(Arg.Any<TmpRoleUser>());
            await _roleUserRefRepository.Received(0).AddAsync(Arg.Any<MdRoleUserRef>());
            await _roleUserRepository.Received(1).AddAsync(Arg.Any<MdRoleUser>());
            await _roleUserRepository.Received(0).GetById(Arg.Any<int>());
            await _roleUserRepository.Received(0).UpdateAsync(Arg.Any<MdRoleUser>());
            await _logRoleUserRefRepository.Received(0).AddAsync(Arg.Any<LgRoleUserRef>());
            await _logRoleUserRepository.Received(1).AddAsync(Arg.Any<LgRoleUser>());
            await _unitOfWorks.Received(2).SaveChangesAsync();
            await _activityRepository.Received(1).AddAsync(Arg.Any<string>());
            await _taskRepository.Received(0).AddAsync(Arg.Any<string>());
            Assert.Equal(SaveStatEnum.Success, result.State);
        }
    }
}
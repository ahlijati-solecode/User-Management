using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using Shared.Constants;
using Shared.Infrastructures.Repositories;
using Shared.Infrastructures.Repositories.TaskList;
using Shared.Infrastructures.Services.User;
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
using User_Management.Infrastructures.Services.Role;
using User_Management.Models.Dtos.Role;
using User_Management.Models.Entities;
using User_Management.Models.Filters;
using Xunit;
using static Shared.Constants.ApiConstants;
using static User_Management.Constants.RoleConstants;
namespace Shared.Service.Tests.Controllers
{
}
namespace UserManagement.Service.Tests.Controllers
{
    public class RoleServiceTest : BaseControllerFactoryTest
    {
        private readonly RoleService _service;
        private readonly IActivityRepository _activityRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly IRoleHisotryRepository _roleHisotryRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWorks _unitOfWorks;
        private readonly ITaskRepository _taskRepository;
        private readonly IRoleAccessRefRepository _roleAccessRefRepository;
        private readonly IRoleUserRefRepository _roleUserRefRepository;

        public RoleServiceTest(WebApiTesterFactory factory) : base(factory)
        {
            _activityRepository = Substitute.For<IActivityRepository>();
            _currentUserService = Substitute.For<ICurrentUserService>();
            _roleHisotryRepository = Substitute.For<IRoleHisotryRepository>();
            _roleRepository = Substitute.For<IRoleRepository>();
            _unitOfWorks = Substitute.For<IUnitOfWorks>();
            _taskRepository = Substitute.For<ITaskRepository>();
            _roleAccessRefRepository = Substitute.For<IRoleAccessRefRepository>();
            _roleUserRefRepository = Substitute.For<IRoleUserRefRepository>();

            _mapper = GenerateMapper();
            _service = new RoleService(_roleRepository,
                _roleHisotryRepository,
                _activityRepository,
                _unitOfWorks,
                _currentUserService,
                _mapper, _taskRepository, _roleAccessRefRepository, _roleUserRefRepository);
        }

        [Fact]
        public async Task Search_Role_By_All()
        {
            // Arrange
            var page = new PagedDto();
            var filter = new RoleFilter();
            filter.ApprovalStatus = "Approved";
            filter.Name = "Test";
            filter.Status = "Active";
            //Act
            var result = await _service.GetPaged(page, filter);

            //Assert
            await _roleRepository.Received(1).GetPaged(Arg.Is<RoleFilter>(n =>
                !string.IsNullOrEmpty(filter.ApprovalStatus) &&
                !string.IsNullOrEmpty(filter.Name) &&
                !string.IsNullOrEmpty(filter.Status)
                ),
          Arg.Is<int>(n => n == 1), Arg.Is<int>(n => n == 10),
          Arg.Is<string>(n => n == "id asc"));
        }

        [Fact]
        public async Task Search_Role_By_Approval_Status()
        {
            // Arrange
            var page = new PagedDto();
            var filter = new RoleFilter();
            filter.ApprovalStatus = "Approved";
            //Act
            var result = await _service.GetPaged(page, filter);

            //Assert
            await _roleRepository.Received(1).GetPaged(Arg.Is<RoleFilter>(n =>
                !string.IsNullOrEmpty(filter.ApprovalStatus) &&
                string.IsNullOrEmpty(filter.Name) &&
                string.IsNullOrEmpty(filter.Status)
                ),
          Arg.Is<int>(n => n == 1), Arg.Is<int>(n => n == 10),
          Arg.Is<string>(n => n == "id asc"));
        }

        [Fact]
        public async Task Search_Role_By_Name()
        {
            // Arrange
            var page = new PagedDto();
            var filter = new RoleFilter();
            filter.Name = "Test";
            //Act
            var result = await _service.GetPaged(page, filter);

            //Assert
            await _roleRepository.Received(1).GetPaged(Arg.Is<RoleFilter>(n =>
                string.IsNullOrEmpty(filter.ApprovalStatus) &&
                !string.IsNullOrEmpty(filter.Name) &&
                string.IsNullOrEmpty(filter.Status)
                ),
          Arg.Is<int>(n => n == 1), Arg.Is<int>(n => n == 10),
          Arg.Is<string>(n => n == "id asc"));
        }

        [Fact]
        public async Task Search_Role_By_Status()
        {
            // Arrange
            var page = new PagedDto();
            var filter = new RoleFilter();
            filter.Status = "Active";
            //Act
            var result = await _service.GetPaged(page, filter);

            //Assert
            await _roleRepository.Received(1).GetPaged(Arg.Is<RoleFilter>(n =>
                string.IsNullOrEmpty(filter.ApprovalStatus) &&
                string.IsNullOrEmpty(filter.Name) &&
                !string.IsNullOrEmpty(filter.Status)
                ),
          Arg.Is<int>(n => n == 1), Arg.Is<int>(n => n == 10),
          Arg.Is<string>(n => n == "id asc"));
        }


        [Fact]
        public async Task View_User_Role_History()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("User-Token", userToken);
            // Act
            var response = await client.GetAsync("user/user-roles?name=Admin_System&sort=id desc&page=1&Size=10");

            // Assert
            var message = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var data = JsonConvert.DeserializeObject(message) as JObject;
            Assert.NotNull(data);
            Assert.NotNull(data["code"]);
            Assert.NotNull(data["data"]);
            Assert.NotNull(data["data"]["items"] as JArray);
            var array = data["data"]["items"] as JArray;
            var roleUserId = 0;
            if (array.Any())
            {
                foreach (var item in array)
                {
                    var child = item as JObject;
                    Assert.NotNull(child["id"]);

                    roleUserId = Convert.ToInt32(child["id"]);
                    break;
                }
                response = await client.GetAsync($"user/user-roles/{roleUserId}/history?Page=1&Size=5&Sort=id%20asc");

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

        [Theory]
        [InlineData("/user/roles")]
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
            Assert.Equal("{\"code\":400,\"status\":\"BadRequest\",\"data\":\"The Name is required\"}", message);
        }

        [Fact]
        public async Task Post_Approved_Success()
        {
            // Arrange
            var id = 1;
            var note = "sample";
            var request = new RoleDto() { Note = note };
            var state = ActivityEnum.Approved;
            var user = new Shared.Models.Core.User()
            {
                UserName = "admin"
            };
            _currentUserService.CurrentUser.Returns(user);
            var mockRole = new MdRole
            {
                Id = id
            };
            _roleRepository.GetById(Arg.Any<int>(), Arg.Any<bool>()).Returns(Task.FromResult(mockRole));
            Tuple<bool, ActivityEnum> mockHistory = new Tuple<bool, ActivityEnum>(false,
                ActivityEnum.Submitted);
            _roleHisotryRepository.CheckHasApproveOrReject(Arg.Any<int>()).Returns(Task.FromResult(mockHistory));
            LgRole log = new LgRole();
            log.Activity = "Submitted";
            _roleHisotryRepository.GetLatestLogByParent(Arg.Any<int>()).Returns(log);
            //Act
            var result = await _service.ApprovalAsync(id, request, state);

            //Assert
            var role = await _roleRepository.Received(1).GetById(Arg.Any<int>(), Arg.Any<bool>());
            var statusApporval = await _roleHisotryRepository.Received(1).CheckHasApproveOrReject(Arg.Any<int>());
            _ = await _roleHisotryRepository.Received(1).AddAsync(Arg.Any<MdRole>(),
                Arg.Is<ActivityEnum>(n => n == ActivityEnum.Approved), Arg.Any<string?>());
            await _activityRepository.Received(1).AddAsync(Arg.Is<string>(n => n.Equals(string.Format(ApprovalMessage,  mockRole.Name, state.ToString()))));
            await _unitOfWorks.Received(1).SaveChangesAsync();
            await _roleRepository.Received(1).UpdateAsync(Arg.Is<MdRole>(n => n.ApprovalStatus == true && n.ApprovedBy != null));
            var curerntUser = _currentUserService.Received(1).CurrentUser;
            Assert.Equal(ApprovalStateEnum.Success, result);
        }

        [Fact]
        public async Task Post_Approved_Success_When_Is_Admin_Active()
        {
            // Arrange
            var id = 1;
            var note = "sample";
            var request = new RoleDto()
            {
                Note = note,
                IsAdmin = true
            };
            var state = ActivityEnum.Approved;
            var user = new Shared.Models.Core.User()
            {
                UserName = "admin"
            };
            _currentUserService.CurrentUser.Returns(user);
            var mockRole = new MdRole
            {
                Id = id,
                MdUserAccesses = new List<MdRoleAccess>()
                {
                    new MdRoleAccess()
                    {
                        Id=1,
                        MdUserAccessRefs = new List<MdRoleAccessRef>()
                        {
                            new MdRoleAccessRef()
                            {
                                IsView=false,
                                Id=1
                            },
                            new MdRoleAccessRef()
                            {
                                IsView=false,
                                Id=2
                            }
                        }
                    },
                    new MdRoleAccess()
                    {
                        Id=2,
                        MdUserAccessRefs = new List<MdRoleAccessRef>()
                        {
                            new MdRoleAccessRef()
                            {
                                IsView=false,
                                Id=3
                            },
                            new MdRoleAccessRef()
                            {
                                IsView=false,
                                Id=4
                            }
                        }
                    }
                }
            };
            _roleRepository.GetById(Arg.Any<int>(), Arg.Any<bool>()).Returns(Task.FromResult(mockRole));
            Tuple<bool, ActivityEnum> mockHistory = new Tuple<bool, ActivityEnum>(false,
                ActivityEnum.Submitted);
            _roleHisotryRepository.CheckHasApproveOrReject(Arg.Any<int>()).Returns(Task.FromResult(mockHistory));
            LgRole log = new LgRole();
            log.IsAdmin = true;
            log.Activity = "Submitted";
            _roleHisotryRepository.GetLatestLogByParent(Arg.Any<int>()).Returns(log);
            //Act
            var result = await _service.ApprovalAsync(id, request, state);

            //Assert
            var role = await _roleRepository.Received(1).GetById(Arg.Any<int>(), Arg.Any<bool>());
            var statusApporval = await _roleHisotryRepository.Received(1).CheckHasApproveOrReject(Arg.Any<int>());
            _ = await _roleHisotryRepository.Received(1).AddAsync(Arg.Any<MdRole>(),
                Arg.Is<ActivityEnum>(n => n == ActivityEnum.Approved), Arg.Any<string?>());
            await _activityRepository.Received(1).AddAsync(Arg.Is<string>(n => n.Equals(string.Format(ApprovalMessage,  mockRole.Name, state.ToString()))));

            await _roleAccessRefRepository.Received(4).UpdateAsync(Arg.Any<MdRoleAccessRef>());


            await _unitOfWorks.Received(1).SaveChangesAsync();
            await _roleRepository.Received(1).UpdateAsync(Arg.Is<MdRole>(n => n.ApprovalStatus == true && n.ApprovedBy != null));
            var curerntUser = _currentUserService.Received(1).CurrentUser;
            Assert.Equal(ApprovalStateEnum.Success, result);
        }

        [Fact]
        public async Task Create_Role_Success()
        {
            // Arrange
            var role = new RoleDto()
            {
                Name = "Role 1",
            };
            var state = ActivityEnum.Approved;

            //Act
            var result = await _service.AddAsync(role);

            //Assert
            _ = await _roleHisotryRepository.Received(1).AddAsync(Arg.Is<LgRole>(n => n.Activity == ActivityEnum.Submitted.ToString()));
            await _activityRepository.Received(1).AddAsync(Arg.Is<string>(n => n.Equals(string.Format(Shared.Constants.ApiConstants.CraetedMessage, TaskListEnum.Role.ToString(), role.Name))));
            await _unitOfWorks.Received(2).SaveChangesAsync();
            await _taskRepository.Received(1).AddAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
            await _roleRepository.Received(1).AddAsync(Arg.Is<MdRole>(n => n.ApprovalStatus == null && n.ApprovalStatus == null));

        }

        [Fact]
        public async Task Delete_Role_Success()
        {
            // Arrange
            var id = 1;
            var state = ActivityEnum.Approved;
            MdRole role = new MdRole()
            {
                Name = "Role 2"
            };
            _roleRepository.GetById(Arg.Any<int>()).Returns(Task.FromResult(role));
            LgRole log = new LgRole();
            log.Activity = "Approved";
            _roleHisotryRepository.GetLatestLogByParent(Arg.Any<int>()).Returns(log);
            //Act
            var result = await _service.DeleteAsync(id);

            //Assert
            _ = await _roleHisotryRepository.Received(1).AddAsync(Arg.Any<LgRole>());
            await _activityRepository.Received(1).AddAsync(Arg.Any<string>());
            await _unitOfWorks.Received(2).SaveChangesAsync();
            _ = await _roleRepository.DeleteAsync(Arg.Is<MdRole>(n => n.Id == 1));
            await _roleRepository.Received(0).AddAsync(Arg.Any<MdRole>());
            await _taskRepository.Received(1).AddAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        }


        [Fact]
        public async Task Post_Approved_Role_NotFound()
        {
            // Arrange
            var id = 1;
            var note = "sample";
            var request = new RoleDto() { Note = note };
            var state = ActivityEnum.Approved;
            MdRole mockRole = null;
            _roleRepository.GetById(Arg.Any<int>()).Returns(Task.FromResult(mockRole));

            //Act
            var result = await _service.ApprovalAsync(id, request, state);

            //Assert
            var role = await _roleRepository.Received(1).GetById(Arg.Any<int>(), Arg.Any<bool>());
            var statusApporval = await _roleHisotryRepository.Received(0).CheckHasApproveOrReject(Arg.Any<int>());
            _ = await _roleHisotryRepository.Received(0).AddAsync(Arg.Any<MdRole>(),
                Arg.Is<ActivityEnum>(n => n == ActivityEnum.Approved), Arg.Any<string?>());
            await _activityRepository.Received(0).AddAsync(Arg.Any<string>());
            await _roleRepository.Received(0).UpdateAsync(Arg.Any<MdRole>());
            await _unitOfWorks.Received(0).SaveChangesAsync();
            var curerntUser = _currentUserService.Received(0).CurrentUser;
            Assert.Equal(ApprovalStateEnum.RoleNotFound, result);
        }

        [Fact]
        public async Task Post_Approved_Has_been_rejected()
        {
            // Arrange
            var id = 1;
            var note = "sample";
            var request = new RoleDto() { Note = note };
            var state = ActivityEnum.Approved;
            var mockRole = new MdRole
            {
                Id = id
            };
            _roleRepository.GetById(Arg.Any<int>(), Arg.Any<bool>()).Returns(Task.FromResult(mockRole));
            Tuple<bool, ActivityEnum> mockHistory = new Tuple<bool, ActivityEnum>(true,
                ActivityEnum.Revised);
            _roleHisotryRepository.CheckHasApproveOrReject(Arg.Any<int>()).Returns(mockHistory);

            //Act
            var result = await _service.ApprovalAsync(id, request, state);

            //Assert
            var role = await _roleRepository.Received(1).GetById(Arg.Any<int>(), Arg.Any<bool>());
            var statusApporval = await _roleHisotryRepository.Received(1).CheckHasApproveOrReject(Arg.Any<int>());
            _ = await _roleHisotryRepository.Received(0).AddAsync(Arg.Any<MdRole>(),
                Arg.Is<ActivityEnum>(n => n == ActivityEnum.Approved), Arg.Any<string?>());

            await _activityRepository.Received(0).AddAsync(Arg.Is<string>(n => n.Equals(string.Format(ApprovalMessage, mockRole.Id, mockRole.Name))));
            await _unitOfWorks.Received(0).SaveChangesAsync();
            await _roleRepository.Received(0).UpdateAsync(Arg.Any<MdRole>());
            Assert.Equal(ApprovalStateEnum.HasProcessed, result);
            var curerntUser = _currentUserService.Received(0).CurrentUser;
        }
    }
}
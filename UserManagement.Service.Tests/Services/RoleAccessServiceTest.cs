using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using Shared.Infrastructures.Repositories;
using Shared.Infrastructures.Repositories.TaskList;
using Shared.Infrastructures.Services;
using Shared.Infrastructures.Services.User;
using Shared.Service.Tests.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using User_Management.Infrastructures.Repositories;
using User_Management.Infrastructures.Repositories.UserAccess;
using User_Management.Infrastructures.Services;
using User_Management.Models.Dtos.User;
using User_Management.Models.Entities;
using Xunit;
using static Shared.Constants.ApiConstants;
using static User_Management.Constants.RoleAccessConstant;

namespace UserManagement.Service.Tests.Controllers
{
    public class RoleAccessServiceTest : BaseControllerFactoryTest
    {
        private readonly RoleAccessService _service;
        private readonly IActivityRepository _activityRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly IRefMenuRepository _refMenuRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWorks _unitOfWorks;
        private readonly IRoleAccessLogRepository _roleAccessLogRepository;
        private readonly IRoleAccessRefLogRepository _roleAccessRefLogRepository;
        private readonly IRoleAccessRefRepository _roleAccessRefRepository;
        private readonly IRoleAccessRepository _roleAccessRepository;
        private readonly ITaskRepository _taskRepository;

        public RoleAccessServiceTest(WebApiTesterFactory factory) : base(factory)
        {
            _activityRepository = Substitute.For<IActivityRepository>();
            _currentUserService = Substitute.For<ICurrentUserService>();
            _mapper = GenerateMapper();
            _refMenuRepository = Substitute.For<IRefMenuRepository>();
            _roleRepository = Substitute.For<IRoleRepository>();
            _unitOfWorks = Substitute.For<IUnitOfWorks>();
            _roleAccessLogRepository = Substitute.For<IRoleAccessLogRepository>();
            _roleAccessRefLogRepository = Substitute.For<IRoleAccessRefLogRepository>();
            _roleAccessRefRepository = Substitute.For<IRoleAccessRefRepository>();
            _roleAccessRepository = Substitute.For<IRoleAccessRepository>();
            _taskRepository = Substitute.For<ITaskRepository>();
            _service = new RoleAccessService(_mapper, _currentUserService,
                _roleAccessRepository, _unitOfWorks,
                _activityRepository, _roleRepository, _refMenuRepository,
                _roleAccessLogRepository, _roleAccessRefLogRepository,
                _roleAccessRefRepository,
                _taskRepository);
        }

        [Theory]
        [InlineData("/user/role-access")]
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
            Assert.Equal("{\"code\":400,\"status\":\"BadRequest\",\"data\":\"The Children field is required.\"}", message);
        }

        [Fact]
        public async Task Post_Update_Role_Access_Not_Found()
        {
            // Arrange
            var id = 2;
            var roleAccess = new RoleAccessDto()
            {
                RoleId = 9,
                MdUserAccessRefs = new List<MdRoleAccessRefDto>()
                {
                    new MdRoleAccessRefDto()
                    {
                        RefMenuId =1,
                    }
                }
            };

            var latestLog = new LgRoleAccessRef();
            _roleAccessRefLogRepository.GetLatestLog(Arg.Any<int>()).Returns(Task.FromResult(latestLog));
            //Act
            var result = await _service.UpdateAsync(id, roleAccess);

            //Assert
            await _roleAccessRepository.Received().GetById(Arg.Is<int>(n => n == 2));
            await _roleRepository.Received(0).GetById(Arg.Is<int>(n => n == 9));
            await _roleAccessLogRepository.Received(0).GetLatestByUserAccessId(Arg.Any<int>());
            await _refMenuRepository.Received(0).GetById(Arg.Any<int>());
            await _roleAccessRefLogRepository.Received(0).GetLatestLog(Arg.Any<int>());
            await _roleAccessRefLogRepository.Received(0).UpdateAsync(Arg.Any<LgRoleAccessRef>());

            await _roleAccessRepository.Received(0).UpdateAsync(Arg.Any<MdRoleAccess>());
            await _activityRepository.Received(0).AddAsync(Arg.Any<string>());
            await _unitOfWorks.Received(0).SaveChangesAsync();
            await _roleAccessLogRepository.Received(0).AddAsync(Arg.Any<LgRoleAccess>());
            await _roleAccessRefLogRepository.Received(0).AddAsync(Arg.Any<LgRoleAccessRef>());
            await _taskRepository.Received(0).AddAsync(Arg.Any<string>(), Arg.Any<Dictionary<string,object>>()); 
            _ = _currentUserService.Received(0).CurrentUser;
            Assert.Equal(UpdateStateEnum.UserAccessNotFound, result.Item1);
        }

        [Fact]
        public async Task Post_Update_Role_Not_Found()
        {
            // Arrange
            var id = 2;
            var roleAccess = new RoleAccessDto()
            {
                RoleId = 9,
                MdUserAccessRefs = new List<MdRoleAccessRefDto>()
                {
                    new MdRoleAccessRefDto()
                    {
                        RefMenuId =1,
                    }
                }
            };

            var dbRoleAccess = new MdRoleAccess()
            {
                RoleId = 9,
                MdUserAccessRefs = new List<MdRoleAccessRef>()
                {
                    new MdRoleAccessRef()
                {
                        RefMenuId=2,
                        Id=6
                }
                }
            };
            _roleAccessRepository.GetById(Arg.Any<int>()).Returns(Task.FromResult(dbRoleAccess));
            var latestLog = new LgRoleAccessRef();
            _roleAccessRefLogRepository.GetLatestLog(Arg.Any<int>()).Returns(Task.FromResult(latestLog));
            //Act
            var result = await _service.UpdateAsync(id, roleAccess);

            //Assert
            await _roleAccessRepository.Received().GetById(Arg.Is<int>(n => n == 2));
            await _roleRepository.Received().GetById(Arg.Is<int>(n => n == 9));
            await _roleAccessLogRepository.Received(0).GetLatestByUserAccessId(Arg.Any<int>());
            await _refMenuRepository.Received(0).GetById(Arg.Any<int>());
            await _roleAccessRefLogRepository.Received(0).GetLatestLog(Arg.Any<int>());
            await _roleAccessRefLogRepository.Received(0).UpdateAsync(Arg.Any<LgRoleAccessRef>());

            await _roleAccessRepository.Received(0).UpdateAsync(Arg.Any<MdRoleAccess>());
            await _activityRepository.Received(0).AddAsync(Arg.Any<string>());
            await _unitOfWorks.Received(0).SaveChangesAsync();
            await _roleAccessLogRepository.Received(0).AddAsync(Arg.Any<LgRoleAccess>());
            await _roleAccessRefLogRepository.Received(0).AddAsync(Arg.Any<LgRoleAccessRef>());
            await _taskRepository.Received(0).AddAsync(Arg.Any<string>(), Arg.Any<Dictionary<string, object>>());
            _ = _currentUserService.Received(0).CurrentUser;
            Assert.Equal(UpdateStateEnum.RoleNotFound, result.Item1);
        }

        [Fact]
        public async Task Post_Update_Menu_Not_Found()
        {
            // Arrange
            var id = 2;
            var roleAccess = new RoleAccessDto()
            {
                RoleId = 9,
                MdUserAccessRefs = new List<MdRoleAccessRefDto>()
                {
                    new MdRoleAccessRefDto()
                    {
                        RefMenuId =1,
                    }
                }
            };

            var dbRoleAccess = new MdRoleAccess()
            {
                RoleId = 9,
                MdUserAccessRefs = new List<MdRoleAccessRef>()
                {
                    new MdRoleAccessRef()
                {
                        RefMenuId=2,
                        Id=6
                }
                }
            };
            _roleAccessRepository.GetById(Arg.Any<int>()).Returns(Task.FromResult(dbRoleAccess));
            var role = new MdRole()
            {
                Id = 9,
            };
            _roleRepository.GetById(Arg.Any<int>()).Returns(Task.FromResult(role));
            var latestLog = new LgRoleAccessRef();
            _roleAccessRefLogRepository.GetLatestLog(Arg.Any<int>()).Returns(Task.FromResult(latestLog));
            //Act
            var result = await _service.UpdateAsync(id, roleAccess);

            //Assert
            await _roleAccessRepository.Received().GetById(Arg.Is<int>(n => n == 2));
            await _roleRepository.Received().GetById(Arg.Is<int>(n => n == 9));
            await _roleAccessLogRepository.Received().GetLatestByUserAccessId(Arg.Is<int>(n => n == 2));
            await _refMenuRepository.Received().GetById(Arg.Is<int>(n => n == 1));
            await _roleAccessRefLogRepository.Received(0).GetLatestLog(Arg.Any<int>());
            await _roleAccessRefLogRepository.Received(0).UpdateAsync(Arg.Any<LgRoleAccessRef>());

            await _roleAccessRepository.Received(0).UpdateAsync(Arg.Any<MdRoleAccess>());
            await _activityRepository.Received(0).AddAsync(Arg.Any<string>());
            await _unitOfWorks.Received(0).SaveChangesAsync();
            await _roleAccessLogRepository.Received(0).AddAsync(Arg.Any<LgRoleAccess>());
            await _roleAccessRefLogRepository.Received(0).AddAsync(Arg.Any<LgRoleAccessRef>());
            await _taskRepository.Received(0).AddAsync(Arg.Any<string>(), Arg.Any<Dictionary<string, object>>());
            _ = _currentUserService.Received(0).CurrentUser;
            Assert.Equal(UpdateStateEnum.MenuNotFound, result.Item1);
        }

        [Fact]
        public async Task Post_Update_Menu_Not_Exists_In_First_Time_Success()
        {
            // Arrange
            var id = 2;
            var roleAccess = new RoleAccessDto()
            {
                RoleId = 9,
                MdUserAccessRefs = new List<MdRoleAccessRefDto>()
                {
                    new MdRoleAccessRefDto()
                    {
                        RefMenuId =1,
                    }
                }
            };
            var menu = new MdRefMenu() { Id = 1, Name = "Template 1" };
            _refMenuRepository.GetById(Arg.Any<int>()).Returns(Task.FromResult(menu));
            var dbRoleAccess = new MdRoleAccess()
            {
                RoleId = 9,
                MdUserAccessRefs = new List<MdRoleAccessRef>()
                {
                    new MdRoleAccessRef()
                {
                        RefMenuId=2,
                        Id=6
                }
                }
            };
            _roleAccessRepository.GetById(Arg.Any<int>()).Returns(Task.FromResult(dbRoleAccess));
            var role = new MdRole()
            {
                Id = 9,
            };
            _roleRepository.GetById(Arg.Any<int>()).Returns(Task.FromResult(role));
            var latestLog = new LgRoleAccessRef();
            _roleAccessRefLogRepository.GetLatestLog(Arg.Any<int>()).Returns(Task.FromResult(latestLog));
            //Act
            var result = await _service.UpdateAsync(id, roleAccess);

            //Assert
            await _roleAccessRepository.Received().GetById(Arg.Is<int>(n => n == 2));
            await _roleRepository.Received().GetById(Arg.Is<int>(n => n == 9));
            await _roleAccessLogRepository.Received().GetLatestByUserAccessId(Arg.Is<int>(n => n == 2));
            await _refMenuRepository.Received().GetById(Arg.Is<int>(n => n == 1));
            await _roleAccessRefLogRepository.Received(0).GetLatestLog(Arg.Any<int>());
            await _roleAccessRefLogRepository.Received(0).UpdateAsync(Arg.Any<LgRoleAccessRef>());

            await _roleAccessRepository.Received().UpdateAsync(Arg.Any<MdRoleAccess>());
            await _activityRepository.Received().AddAsync(Arg.Any<string>());
            await _unitOfWorks.Received().SaveChangesAsync();
            await _roleAccessLogRepository.Received(0).AddAsync(Arg.Any<LgRoleAccess>());
            await _roleAccessRefLogRepository.Received(1).AddAsync(Arg.Any<LgRoleAccessRef>());
            _ = _currentUserService.Received(1).CurrentUser;
            Assert.Equal(UpdateStateEnum.Success, result.Item1);
        }

        [Fact]
        public async Task Create_Role_Permission_Success()
        {
            // Arrange
            var id = 2;
            var roleAccess = new RoleAccessDto()
            {
                RoleId = 9,
                MdUserAccessRefs = new List<MdRoleAccessRefDto>()
                {
                    new MdRoleAccessRefDto()
                    {
                        RefMenuId =1,
                    }
                }
            };
            var menu = new MdRefMenu() { Id = 1, Name = "Template 1" };
            _refMenuRepository.GetById(Arg.Any<int>()).Returns(Task.FromResult(menu));
            var menus = new List<MdRefMenu>();
            menus.Add(new MdRefMenu() { Id = 2, Name = "Template 2" });
            _refMenuRepository.GetAllMenuData().Returns(menus);
            var role = new MdRole()
            {
                Id = 9,
                IsAdmin=true
            };
            _roleRepository.GetById(Arg.Any<int>()).Returns(Task.FromResult(role));

            //Act
            var result = await _service.AddAsync(roleAccess);

            //Assert
            await _roleAccessRepository.Received(0).GetById(Arg.Is<int>(n => n == 2));
            await _refMenuRepository.Received(1).GetAllMenuData(); 
            await _roleRepository.Received().GetById(Arg.Is<int>(n => n == 9));
            await _roleAccessLogRepository.Received(0).GetLatestByUserAccessId(Arg.Any<int>());
            await _refMenuRepository.Received().GetById(Arg.Is<int>(n => n == 1));
            await _roleAccessRefLogRepository.Received(0).GetLatestLog(Arg.Any<int>());
            await _roleAccessRefLogRepository.Received(0).UpdateAsync(Arg.Any<LgRoleAccessRef>());

            await _roleAccessRepository.Received(0).UpdateAsync(Arg.Any<MdRoleAccess>());
            await _roleAccessRepository.Received(1).AddAsync(Arg.Is<MdRoleAccess>(n=>n.IsActive ==false));
            await _activityRepository.Received().AddAsync(Arg.Any<string>());
            await _unitOfWorks.Received().SaveChangesAsync();
            await _roleAccessLogRepository.Received(1).AddAsync(Arg.Any<LgRoleAccess>());
            await _roleAccessRefLogRepository.Received(2).AddAsync(Arg.Any<LgRoleAccessRef>());

            Assert.Equal(AddStateEnum.Success, result.Item1);
        }

        [Fact]
        public async Task Post_Update_Success()
        {
            // Arrange
            var id = 2;
            var roleAccess = new RoleAccessDto()
            {
                RoleId = 9,
                MdUserAccessRefs = new List<MdRoleAccessRefDto>()
                {
                    new MdRoleAccessRefDto()
                    {
                        RefMenuId =1,
                    }
                }
            };
            var menu = new MdRefMenu() { Id = 1, Name = "Template 1" };
            _refMenuRepository.GetById(Arg.Any<int>()).Returns(Task.FromResult(menu));
            var dbRoleAccess = new MdRoleAccess()
            {
                RoleId = 9,
                MdUserAccessRefs = new List<MdRoleAccessRef>()
                {
                    new MdRoleAccessRef()
                {
                        RefMenuId=1,
                        Id=6
                }
                }
            };
            _roleAccessRepository.GetById(Arg.Any<int>()).Returns(Task.FromResult(dbRoleAccess));
            var role = new MdRole()
            {
                Id = 9,
            };
            _roleRepository.GetById(Arg.Any<int>()).Returns(Task.FromResult(role));
            var latestLog = new LgRoleAccessRef();
            _roleAccessRefLogRepository.GetLatestLog(Arg.Any<int>()).Returns(Task.FromResult(latestLog));

            //Act
            var result = await _service.UpdateAsync(id, roleAccess);

            //Assert
            await _roleAccessRepository.Received().GetById(Arg.Is<int>(n => n == 2));
            await _roleRepository.Received().GetById(Arg.Is<int>(n => n == 9));
            await _roleAccessLogRepository.Received().GetLatestByUserAccessId(Arg.Is<int>(n => n == 2));
            await _refMenuRepository.Received().GetById(Arg.Is<int>(n => n == 1));
            await _roleAccessRefLogRepository.Received().GetLatestLog(Arg.Is<int>(n => n == 6));
            await _roleAccessRefLogRepository.Received().UpdateAsync(Arg.Any<LgRoleAccessRef>());

            await _roleAccessRepository.Received().UpdateAsync(Arg.Any<MdRoleAccess>());
            await _activityRepository.Received().AddAsync(Arg.Any<string>());
            await _unitOfWorks.Received().SaveChangesAsync();
            await _roleAccessLogRepository.Received(0).AddAsync(Arg.Any<LgRoleAccess>());
            await _roleAccessRefLogRepository.Received(0).AddAsync(Arg.Any<LgRoleAccessRef>());

            Assert.Equal(UpdateStateEnum.Success, result.Item1);
        }

        [Fact]
        public async Task Update_With_Approved_Data_Success()
        {
            // Arrange
            var id = 2;
            var roleAccess = new RoleAccessDto()
            {
                RoleId = 9,
                MdUserAccessRefs = new List<MdRoleAccessRefDto>()
                {
                    new MdRoleAccessRefDto()
                    {
                        RefMenuId =1,
                    }
                }
            };
            var menu = new MdRefMenu() { Id = 1, Name = "Template 1" };
            _refMenuRepository.GetById(Arg.Any<int>()).Returns(Task.FromResult(menu));
            var dbRoleAccess = new MdRoleAccess()
            {
                RoleId = 9,
                MdUserAccessRefs = new List<MdRoleAccessRef>()
                {
                    new MdRoleAccessRef()
                {
                        RefMenuId=1,
                        Id=6
                }
                }
            };
            _roleAccessRepository.GetById(Arg.Any<int>()).Returns(Task.FromResult(dbRoleAccess));
            var role = new MdRole()
            {
                Id = 9,
                IsAdmin=true
            };
            _roleRepository.GetById(Arg.Any<int>()).Returns(Task.FromResult(role));
            var latestLog = new LgRoleAccessRef();
            _roleAccessRefLogRepository.GetLatestLog(Arg.Any<int>()).Returns(Task.FromResult(latestLog));
            LgRoleAccess lgRoleAccess = new LgRoleAccess()
            {
                Activity = ActivityEnum.Approved.ToString()
            };
            _roleAccessLogRepository.GetLatestByUserAccessId(Arg.Any<int>()).Returns(Task.FromResult(lgRoleAccess));

            var menus = new List<MdRefMenu>();
            menus.Add(new MdRefMenu() { Id = 2, Name = "Template 2" });
            _refMenuRepository.GetAllMenuData().Returns(menus);


            //Act
            var result = await _service.UpdateAsync(id, roleAccess);

            //Assert
            await _roleAccessRepository.Received().GetById(Arg.Is<int>(n => n == 2));
            await _roleRepository.Received().GetById(Arg.Is<int>(n => n == 9));
            await _roleAccessLogRepository.Received().GetLatestByUserAccessId(Arg.Is<int>(n => n == 2));
            await _refMenuRepository.Received().GetById(Arg.Is<int>(n => n == 1));
            await _roleAccessRefLogRepository.Received(0).GetLatestLog(Arg.Any<int>());
            await _roleAccessRefLogRepository.Received(0).UpdateAsync(Arg.Any<LgRoleAccessRef>());

            await _roleAccessRepository.Received().UpdateAsync(Arg.Any<MdRoleAccess>());
            await _activityRepository.Received().AddAsync(Arg.Any<string>());
            await _unitOfWorks.Received().SaveChangesAsync();
            await _roleAccessLogRepository.Received(1).AddAsync(Arg.Any<LgRoleAccess>());
            await _roleAccessRefLogRepository.Received(2).AddAsync(Arg.Any<LgRoleAccessRef>());

            Assert.Equal(UpdateStateEnum.Success, result.Item1);
        }

        [Fact]
        public async Task Update_With_Submited_Data_Failed()
        {
            // Arrange
            var id = 2;
            var roleAccess = new RoleAccessDto()
            {
                RoleId = 9,
                MdUserAccessRefs = new List<MdRoleAccessRefDto>()
                {
                    new MdRoleAccessRefDto()
                    {
                        RefMenuId =1,
                    }
                }
            };
            var menu = new MdRefMenu() { Id = 1, Name = "Template 1" };
            _refMenuRepository.GetById(Arg.Any<int>()).Returns(Task.FromResult(menu));
            var dbRoleAccess = new MdRoleAccess()
            {
                RoleId = 9,
                MdUserAccessRefs = new List<MdRoleAccessRef>()
                {
                    new MdRoleAccessRef()
                {
                        RefMenuId=1,
                        Id=6
                }
                }
            };
            _roleAccessRepository.GetById(Arg.Any<int>()).Returns(Task.FromResult(dbRoleAccess));
            var role = new MdRole()
            {
                Id = 9,
            };
            _roleRepository.GetById(Arg.Any<int>()).Returns(Task.FromResult(role));
            var latestLog = new LgRoleAccessRef();
            _roleAccessRefLogRepository.GetLatestLog(Arg.Any<int>()).Returns(Task.FromResult(latestLog));
            LgRoleAccess lgRoleAccess = new LgRoleAccess()
            {
                Activity = ActivityEnum.Submitted.ToString()
            };
            _roleAccessLogRepository.GetLatestByUserAccessId(Arg.Any<int>()).Returns(Task.FromResult(lgRoleAccess));
            //Act
            var result = await _service.UpdateAsync(id, roleAccess);

            //Assert
            await _roleAccessRepository.Received().GetById(Arg.Is<int>(n => n == 2));
            await _roleRepository.Received().GetById(Arg.Is<int>(n => n == 9));
            await _roleAccessLogRepository.Received().GetLatestByUserAccessId(Arg.Is<int>(n => n == 2));
            await _refMenuRepository.Received(0).GetById(Arg.Any<int>());
            await _roleAccessRefLogRepository.Received(0).GetLatestLog(Arg.Any<int>());
            await _roleAccessRefLogRepository.Received(0).UpdateAsync(Arg.Any<LgRoleAccessRef>());

            await _roleAccessRepository.Received(0).UpdateAsync(Arg.Any<MdRoleAccess>());
            await _activityRepository.Received(0).AddAsync(Arg.Any<string>());
            await _unitOfWorks.Received(0).SaveChangesAsync();
            await _roleAccessLogRepository.Received(0).AddAsync(Arg.Any<LgRoleAccess>());
            await _roleAccessRefLogRepository.Received(0).AddAsync(Arg.Any<LgRoleAccessRef>());

            Assert.Equal(UpdateStateEnum.WaitingApprovedByApproval, result.Item1);
        }

        [Fact]
        public async Task View_UserAccess_History()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("User-Token", userToken);
            // Act
            var response = await client.GetAsync("user/role-access?sort=id desc&page=1&Size=10");

            // Assert
            var message = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var data = JsonConvert.DeserializeObject(message) as JObject;
            Assert.NotNull(data);
            Assert.NotNull(data["code"]);
            Assert.NotNull(data["data"]);
            Assert.NotNull(data["data"]["items"] as JArray);
            var array = data["data"]["items"] as JArray;
            var roleAccessId = 0;
            if (array.Any())
            {
                foreach (var item in array)
                {
                    var child = item as JObject;
                    Assert.NotNull(child["id"]);

                    roleAccessId = Convert.ToInt32(child["id"]);
                    break;
                }
                response = await client.GetAsync($"user/role-access/{roleAccessId}/histories?Page=1&Size=5&Sort=id%20asc");

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
    }
}
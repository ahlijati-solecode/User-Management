using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using Shared.Infrastructures.Repositories;
using Shared.Models.Core;
using Shared.Models.Dtos.Core;
using Shared.Models.Entities;
using Shared.Models.Filters;
using Shared.Service.Tests.Controllers;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using User_Management.Infrastructures.Services.AcitivityLogs;
using User_Management.Infrastructures.Utils.Report;
using Xunit;

namespace UserManagement.Service.Tests.Controllers
{
    public class ActivityServiceTest : BaseControllerFactoryTest
    {
        private readonly ActivityService _service;
        private readonly IActivityRepository _activityRepository;
        private readonly IMapper _mapper;
        private readonly IActivityReportProvider _reportProvider;

        public ActivityServiceTest(WebApiTesterFactory factory) : base(factory)
        {
            _activityRepository = Substitute.For<IActivityRepository>();
            _reportProvider = Substitute.For<IActivityReportProvider>();
            _mapper = GenerateMapper();
            _service = new ActivityService(_activityRepository, _mapper, _reportProvider);
        }
        [Fact]
        public async Task Post_Export_Success()
        {
            // Arrange
            var page = new PagedDto()
            {
                Page = 2,
                Size = 10
            };
            var filter = new ActivityFilter();
            var paged = new Paged<LgActivity>()
            {
                Items = new List<LgActivity>(),
                TotalItems = 0
            };
            _activityRepository.GetPaged(Arg.Any<PageFilter>(), Arg.Any<ActivityFilter>()).Returns(paged);
            //Act
            var result = await _service.ExportAsync(page, filter);

            //Assert
            _ = await _activityRepository.Received(1).GetPaged(Arg.Is<PageFilter>(n => n.Page == 1 && n.Size == int.MaxValue), Arg.Any<ActivityFilter>());
            _ = await _reportProvider.Received(1).GenerateReportAsync(Arg.Any<IEnumerable<LgActivity>>());
        }

        [Fact]
        public async Task View_User_Management_History()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("user/log-activity?page=1&size=10&sort=time desc");

            // Assert
            var message = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var data = JsonConvert.DeserializeObject(message) as JObject;
            Assert.NotNull(data);
            Assert.NotNull(data["code"]);
            Assert.NotNull(data["data"]);
            Assert.NotNull(data["data"] as JArray);
            var array = data["data"] as JArray;
            foreach (var item in array)
            {
                var child = item as JObject;
                Assert.NotNull(child["username"]);
                Assert.NotNull(child["activity"]);
                Assert.NotNull(child["time"]);

            }
          
        }


    }
}
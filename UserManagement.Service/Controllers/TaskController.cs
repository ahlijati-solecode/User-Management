using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Shared.Configurations.Attributes;
using Shared.Models.Dtos.Core;
using Shared.Models.Requests;
using Shared.Service.Infrastructures.Services.Task;
using User_Management.Infrastructures.Services.Tasks;

namespace User_Management.Controllers
{
    [Authorize]
    [Route("user/task-list")]
    [ApiController]
    public class TaskController : ApiController
    {
        private readonly ITaskCountService _taskCountService;
        private readonly ITaskService _taskService;
        private readonly IMapper _mapper;

        public TaskController(ITaskCountService taskCountService, 
            ITaskService taskService,
            IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _taskCountService = taskCountService;
            _taskService = taskService;
            _mapper = mapper;
        }
        [HttpGet("count")]
        public async Task<IActionResult> GetStatusRoleAsync()
        {
            return Ok(await _taskCountService.CountAsync());
        }
        [HttpGet()]
        public async Task<IActionResult> GetPagedHistoryAsync([FromQuery] BasePagedRequest request)
        {
            var result = await _taskService.GePaged(_mapper.Map<PagedDto>(request));
            return Ok(result);
        }
    }
}
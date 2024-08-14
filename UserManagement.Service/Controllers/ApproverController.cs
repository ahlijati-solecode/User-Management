using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Shared.Configurations.Attributes;
using Shared.Models.Dtos.Core;
using Shared.Models.Requests;
using User_Management.Infrastructures.Services;
using User_Management.Models.Responses.UserAccess;

namespace User_Management.Controllers
{
    [Authorize]
    [Route("user/approver")]
    [ApiController]
    public class ApproverController : ApiController
    {
        private readonly IMapper _mapper;
        private readonly IRoleUserService _roleUserService;

        public ApproverController(
            IMapper mapper,
            IRoleUserService roleUserService,IHttpContextAccessor httpContextAccessor):base(httpContextAccessor)
        {
            _mapper = mapper;
            _roleUserService = roleUserService;
        }
        [HttpGet()]
        public async Task<IActionResult> GetStatusRoleAsync()
        {
            return Ok(_mapper.Map<List<UserResponse>>(await _roleUserService.GetAllApproverAsync()));
        }

    }
}
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Shared.Configurations.Attributes;
using Shared.Constants;
using Shared.Models.Core;
using Shared.Models.Dtos.Core;
using Shared.Models.Requests;
using static Shared.Constants.ApiConstants;
using User_Management.Models.Responses.Role;
using User_Management.Models.Requests.Role;
using static User_Management.Constants.RoleConstants;
using User_Management.Models.Dtos.Role;
using User_Management.Models.Filters;
using User_Management.Infrastructures.Services.Role;

namespace User_Management.Controllers
{
    [Authorize]
    [Route("user/roles")]
    [ApiController]
    public class LgRoleController : ApiController
    {
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public LgRoleController(IRoleService roleService,
            IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _roleService = roleService;
            _mapper = mapper;
        }
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveAsync([FromQuery] RolePageRequest request)
        {
            var result = await _roleService.GetActiveAsync(_mapper.Map<PagedDto>(request), _mapper.Map<RoleFilter>(request));
            return Ok(result);
        }
        [HttpPost("{id}/approval/{state}")]
        public async Task<IActionResult> ApprovalAsync(int id, [FromBody] ApprovalRequest? request, ActivityEnum state)
        {
            var result = await _roleService.ApprovalAsync(id, _mapper.Map<RoleDto>(request), state);
            switch (result)
            {
                case ApprovalStateEnum.RoleNotFound:
                    return NotFound(string.Format(NotExitsMessage, id));

                case ApprovalStateEnum.HasProcessed:
                    Message = string.Format(Shared.Constants.ApiConstants.StatusApprovalMessage, _roleService.CurrrentActivity.ToString());
                    return Ok(false);
            }
            Message = state == ActivityEnum.Approved ? ApiConstants.DataHasBeenApprovedMessage : ApiConstants.DataHasBeenRejectedMessage;
            return Ok(true);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            Message = ApiConstants.DeleteFailedMessage;
            var result = await _roleService.DeleteAsync(id);
            switch (result)
            {
                case DeleteStateEnum.RoleNotFound:
                    return NotFound(ApiConstants.DeleteFailedMessage + " - " + string.Format(NotExitsMessage, id));

                case DeleteStateEnum.WaitingApprovedByApproval:
                    return BadRequest(ApiConstants.DeleteFailedMessage + " - " + string.Format(ApiConstants.NeedProccedByApproval));
                case DeleteStateEnum.NotFound:
                    return NotFound(ApiConstants.DeleteFailedMessage + " - " + string.Format(NotExitsMessage, id));
                case DeleteStateEnum.UsedInRoleAccess:
                    return BadRequest(ApiConstants.DeleteFailedMessage + " - " + string.Format(ConnectedToAccessControlListMessage));
                case DeleteStateEnum.UsedInRoleUser:
                    return BadRequest(ApiConstants.DeleteFailedMessage + " - " + string.Format(ConnectedToUserRoleMessage));
                case DeleteStateEnum.UsedInRoleAccessAndUserRole:
                    return BadRequest(ApiConstants.DeleteFailedMessage + " - " + string.Format(ConnectedToUserRoleAndAccessControlListMessage));
                default:
                    break;
            }
            Message = ApiConstants.DeleteSuccessMessage;
            return Ok(true);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var role = await _roleService.GetAsync(id);
            if (role.State == GetByIdStateEnum.RoleNotFound)
                return NotFound(string.Format(NotExitsMessage, id));
            return Ok(_mapper.Map<RoleResponse>(role.Data));
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] CreateRoleRequest request)
        {
            Message = ApiConstants.InsertFailedMessage;
            var role = _mapper.Map<RoleDto>(request);
            var result = await _roleService.AddAsync(role);
            switch (result.State)
            {
                case AddStateEnum.DuplicateRoleName:
                    return BadRequest(ApiConstants.InsertFailedMessage + " - " + string.Format(ApiConstants.DuplicateName, request.Name, "role"));
            }
            Message = ApiConstants.InsertSuccessMessage;
            return Ok(_mapper.Map<RoleResponse>(result.Data));
        }

        [HttpGet()]
        public async Task<IActionResult> GetPagedAsync([FromQuery] RolePageRequest request)
        {
            var items = await _roleService.GetPaged(_mapper.Map<PagedDto>(request), _mapper.Map<RoleFilter>(request));
            return Ok(_mapper.Map<Paged<MdRolePagedResponse>>(items));
        }

        [HttpGet("{id}/histories")]
        public async Task<IActionResult> GetPagedHistoryAsync(int id, [FromQuery] RoleHistoryPagedRequest request)
        {
            var result = await _roleService.GePagedHistories(id, _mapper.Map<PagedDto>(request));
            if (result.State == GetRoleHistoiresEnum.RoleNotFound)
                return NotFound(string.Format(NotExitsMessage, id));
            var response = GenerateNumber(_mapper.Map<Paged<LgRoleResponse>>(result.Data), request);
            return Ok(response);
        }

        private Paged<LgRoleResponse> GenerateNumber(Paged<LgRoleResponse> response, RoleHistoryPagedRequest request)
        {
            var index = 1;
            foreach (var item in response.Items)
            {
                item.No = index + ((request.Page - 1) * request.Size);
                index++;
            }
            return response;
        }

        [HttpPut]
        public async Task<IActionResult> PutAsync([FromBody] EditeRoleRequest request)
        {
            Message = ApiConstants.UpdateFailedMessage;
            var result = await _roleService.UpdateAsync(request.Id, _mapper.Map<RoleDto>(request));
            switch (result.State)
            {
                case UpdateStateEnum.RoleNotFound:
                    return NotFound(ApiConstants.UpdateFailedMessage + " - " + string.Format(NotExitsMessage, request.Id));
                case UpdateStateEnum.NotFound:
                    return NotFound(ApiConstants.UpdateFailedMessage + " - " + string.Format(NotExitsMessage, request.Id));
                case UpdateStateEnum.DuplicateRoleName:
                    return BadRequest(ApiConstants.UpdateFailedMessage + " - " + string.Format(ApiConstants.DuplicateName, request.Name, "role"));
                case UpdateStateEnum.WaitingApprovedByApproval:
                    return BadRequest(ApiConstants.UpdateFailedMessage + " - " + NeedProccedByApproval);
            }

            Message = ApiConstants.UpdateSuccessMessage;
            return Ok(_mapper.Map<RoleResponse>(result.Data));
        }
    }
}
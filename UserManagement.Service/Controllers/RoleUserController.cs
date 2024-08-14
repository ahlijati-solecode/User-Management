using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Shared.Configurations.Attributes;
using Shared.Constants;
using Shared.Models.Core;
using Shared.Models.Dtos.Core;
using Shared.Models.Requests;
using User_Management.Infrastructures.Services;
using User_Management.Models.Dtos.UserRole;
using User_Management.Models.Filters;
using User_Management.Models.Requests.RoleUser;
using User_Management.Models.Responses.RoleUser;
using User_Management.Models.Responses.UserRole;
using static Shared.Constants.ApiConstants;
using static User_Management.Constants.RoleUserConstants;

namespace User_Management.Controllers
{
    [Authorize]
    [Route("user/user-roles")]
    [ApiController]
    public class RoleUserController : ApiController
    {
        private readonly IRoleUserService _roleUserService;
        private readonly IMapper _mapper;

        public RoleUserController(IRoleUserService roleUserService,
            IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _roleUserService = roleUserService;
            _mapper = mapper;
        }
        [HttpPost("key")]
        public async Task<IActionResult> CreateUniqueKeyAsync([FromBody] CreateUniqueKeyRequest request)
        {
            var result = await _roleUserService.CreateUniqueAsync(request.RoleUserId, request.JustView);
            switch (result.State)
            {
                case CreateUniqueStateEnum.RoleUserNotFound:
                    return NotFound(string.Format(NotExitsMessage, request.RoleUserId));

                case CreateUniqueStateEnum.WaitingApprovedByApproval:
                    return BadRequest(string.Format(ApiConstants.NeedProccedByApproval));
                default:
                    break;
            }
            return Ok(result.Data);
        }

        [HttpPost("add/{uniqueId}")]
        public async Task<IActionResult> AddRoleUserAsync(string uniqueId, [FromBody] AddRoleUserRequest request)
        {
            Message = ApiConstants.InsertFailedMessage;
            var result = await _roleUserService.AddRoleUserAsync(uniqueId, _mapper.Map<RoleUserDto>(request));
            switch (result.State)
            {
                case AddRoleUserTemporaryStatEnum.ParentNotFound:
                    return NotFound(ApiConstants.InsertFailedMessage + " - " + string.Format(NotExitsMessage, uniqueId));
                case AddRoleUserTemporaryStatEnum.RoleNotFound:
                    return NotFound(ApiConstants.InsertFailedMessage + " - " + string.Format(RoleNotExitsMessage, request.RoleId));
                case AddRoleUserTemporaryStatEnum.UserNotFound:
                    return NotFound(ApiConstants.InsertFailedMessage + " - " + string.Format(ApiConstants.NoExistMessage, "User", request.Username));
                case AddRoleUserTemporaryStatEnum.DuplicateRole:
                    return BadRequest(ApiConstants.InsertFailedMessage + " - " + string.Format(ApiConstants.DuplicateName, result.Data, "Role"));
                case AddRoleUserTemporaryStatEnum.DuplicateUser:
                    return BadRequest(ApiConstants.InsertFailedMessage + " - " + string.Format(ApiConstants.DuplicateName, request.Username, "User"));
                default:
                    break;
            }
            Message = ApiConstants.InsertSuccessMessage;
            return Ok(result.Data);
        }

        [HttpPut("add/{uniqueId}/{id}")]
        public async Task<IActionResult> UpdateRoleUserAsync(string uniqueId, string id, [FromBody] AddRoleUserRequest request)
        {
            Message = ApiConstants.UpdateFailedMessage;
            var result = await _roleUserService.UpdateRoleUserAsync(uniqueId, id, _mapper.Map<RoleUserDto>(request));
            switch (result.State)
            {
                case UpdateRoleUserTemporaryStatEnum.ParentNotFound:
                    return NotFound(ApiConstants.UpdateFailedMessage + " - " + string.Format(NotExitsMessage, uniqueId));
                case UpdateRoleUserTemporaryStatEnum.RoleNotFound:
                    return NotFound(ApiConstants.UpdateFailedMessage + " - " + string.Format(RoleNotExitsMessage, request.RoleId));
                case UpdateRoleUserTemporaryStatEnum.ItemNotFound:
                    return NotFound(ApiConstants.UpdateFailedMessage + " - " + string.Format(ApiConstants.NoExistMessage, "User Role", id));
                case UpdateRoleUserTemporaryStatEnum.UserNotFound:
                    return NotFound(ApiConstants.UpdateFailedMessage + " - " + string.Format(ApiConstants.NoExistMessage, "User", request.Username));
                case UpdateRoleUserTemporaryStatEnum.DuplicateUser:
                    return BadRequest(ApiConstants.UpdateFailedMessage + " - " + string.Format(ApiConstants.DuplicateName, request.Username, "User"));
                default:
                    break;
            }
            Message = ApiConstants.UpdateSuccessMessage;
            return Ok(result.Data);
        }

        [HttpDelete("add/{uniqueId}/{id}")]
        public async Task<IActionResult> DeleteRoleUserAsync(string uniqueId, string id)
        {
            var result = await _roleUserService.DeleteRoleUserAsync(uniqueId, id);
            switch (result.State)
            {
                case DeletedRoleUserTemporaryStatEnum.ParentNotFound:
                    return NotFound(string.Format(NotExitsMessage, uniqueId));
                case DeletedRoleUserTemporaryStatEnum.ItemNotFound:
                    return NotFound(string.Format(ApiConstants.NoExistMessage, "User Role", id));

                default:
                    break;
            }
            Message = ApiConstants.DeleteSuccessMessage;
            return Ok(result.Data);
        }

        [HttpGet("add/{uniqueId}")]
        public async Task<IActionResult> GetPagedRoleUserAsync(string uniqueId, [FromQuery] RoleUserPageRequest request)
        {
            var result = await _roleUserService.GetPagedRoleUserAsync(uniqueId, _mapper.Map<PagedDto>(request), _mapper.Map<RoleUserFilter>(request));
            return Ok(_mapper.Map<Paged<TmpRoleUserReResponse>>(result));
        }

        [HttpPost("add/{uniqueId}/commit")]
        public async Task<IActionResult> CommitRoleUserAsync(string uniqueId, [FromBody] CommitRoleUserRequest request)
        {
            Message = ApiConstants.InsertFailedMessage;
            var result = await _roleUserService.CommitRoleUserAsync(uniqueId, _mapper.Map<RoleUserDto>(request));
            switch (result.State)
            {
                case SaveStatEnum.ParentNotFound:
                    return NotFound(ApiConstants.InsertFailedMessage + " - " + string.Format(NotExitsMessage, uniqueId));
                case SaveStatEnum.ItemNotFound:
                    return NotFound(ApiConstants.InsertFailedMessage + " - " + string.Format(ApiConstants.NoExistMessage, "User Role", request.RoleUserId));
                default:
                    break;
            }
            Message = ApiConstants.InsertSuccessMessage;
            return Ok(result.Data);
        }

        [HttpPut("add/{uniqueId}/commit")]
        public async Task<IActionResult> CommitRoleUserAsync(string uniqueId, [FromBody] EditRoleUserRequest request)
        {
            Message = ApiConstants.InsertFailedMessage;
            var result = await _roleUserService.CommitRoleUserAsync(uniqueId, _mapper.Map<RoleUserDto>(request));
            switch (result.State)
            {
                case SaveStatEnum.ParentNotFound:
                    return NotFound(string.Format(NotExitsMessage, uniqueId));
                case SaveStatEnum.ItemNotFound:
                    return NotFound(string.Format(ApiConstants.NoExistMessage, "User Role", request.RoleUserId));
                default:
                    break;
            }
            Message = ApiConstants.UpdateSuccessMessage;
            return Ok(result.Data);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAsync(int id)
        {
            Message = ApiConstants.DeleteFailedMessage;
            var result = await _roleUserService.DeleteUserAsync(id);
            switch (result.State)
            {
                case DeleteStateEnum.ItemNotFound:
                    return NotFound(ApiConstants.DeleteFailedMessage + " - " + string.Format(ApiConstants.NoExistMessage, "User Role", id));
                case DeleteStateEnum.WaitingApprovedByApproval:
                    return BadRequest(ApiConstants.DeleteFailedMessage + " - " + string.Format(ApiConstants.NeedProccedByApproval)); break;
                default:
                    break;
            }
            Message = ApiConstants.DeleteSuccessMessage;
            return Ok(result.Data);
        }
        [HttpPost("{id}/approval/{state}")]
        public async Task<IActionResult> ApprovalAsync(int id, [FromBody] ApprovalRequest request, ActivityEnum state)
        {
            var result = await _roleUserService.ApprovedAsync(id, state, request.Note);
            switch (result.State)
            {
                case ApprovedStateEnum.ItemNotFound:
                    return NotFound(string.Format(ApiConstants.NoExistMessage, "User Role", id));
                case ApprovedStateEnum.HasBeenProcessed:
                    return BadRequest(string.Format(ApiConstants.ProcessedSuccessMessage));
                default:
                    break;
            }
            Message = state == ActivityEnum.Approved ? ApiConstants.DataHasBeenApprovedMessage : ApiConstants.DataHasBeenRejectedMessage;
            return Ok(result.Data);
        }
        [HttpGet()]
        public async Task<IActionResult> GetPagedAsync([FromQuery] RoleUserPageRequest request)
        {
            var items = await _roleUserService.GetPaged(_mapper.Map<PagedDto>(request), _mapper.Map<RoleUserFilter>(request));
            return Ok(items);
        }

        [HttpGet("{id}/history")]
        public async Task<IActionResult> GetPagedHistoryAsync(int id, [FromQuery] RoleUserHistoryPageRequest request)
        {
            var result = await _roleUserService.GePagedHistories(id, _mapper.Map<PagedDto>(request));
            if (result.State == GetPagedUserRoleHistoryStateEnum.ItemNotFound)
                return NotFound(string.Format(NotExitsMessage, id));
            var response = GenerateNumber(_mapper.Map<Paged<RoleUserHistoryResponse>>(result.Data), request);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await _roleUserService.GetPreviewByIdAsync(id);
            if (result.State == GetByIdUserRoleHistoryStateEnum.ItemNotFound)
                return NotFound(string.Format(NotExitsMessage, id));
            return Ok(result.Data);
        }


        private Paged<RoleUserHistoryResponse> GenerateNumber(Paged<RoleUserHistoryResponse> response, RoleUserHistoryPageRequest request)
        {
            var index = 1;
            foreach (var item in response.Items)
            {
                item.No = index + ((request.Page - 1) * request.Size);
                index++;
            }
            return response;
        }

    }
}
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Shared.Configurations.Attributes;
using Shared.Models.Core;
using Shared.Models.Dtos.Core;
using Shared.Models.Requests;
using Shared.Constants;
using Shared.Infrastructures.Services;
using static Shared.Constants.ApiConstants;
using User_Management.Infrastructures.Services;
using User_Management.Models.Requests.UserAccess;
using User_Management.Models.Dtos.User;
using static User_Management.Constants.RoleAccessConstant;
using User_Management.Constants;
using User_Management.Models.Responses.RoleAccess;
using User_Management.Models.Filters;

namespace User_Management.Controllers
{
    [Authorize]
    [Route("user/role-access")]
    [ApiController]
    public class RoleAccessController : ApiController
    {
        private readonly IMapper _mapper;
        private readonly IRoleAccessService _roleccessService;

        public RoleAccessController(
            IMapper mapper,
            IRoleAccessService roleAccessService, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _mapper = mapper;
            _roleccessService = roleAccessService;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] BaseUserAccessRequest request)
        {
            Message = InsertFailedMessage;
            var role = _mapper.Map<RoleAccessDto>(request);
            var result = await _roleccessService.AddAsync(role);
            switch (result.Item1)
            {
                case AddStateEnum.RoleNotFound:
                    return NotFound(ApiConstants.InsertFailedMessage + " - " + string.Format(RoleConstants.NotExitsMessage, request.RoleId));
                case AddStateEnum.MenuNotFound:
                    return NotFound(ApiConstants.InsertFailedMessage + " - " + MenuConstants.DataNotFound);
                case AddStateEnum.DuplicateRole:
                    return BadRequest(ApiConstants.InsertFailedMessage + " - " + string.Format(ApiConstants.DuplicateName, result.Item2.Name, "Role"));
            }
            Message = InsertSuccessMessage;
            return Ok(_mapper.Map<RoleAccessResponse>(result.Item2));
        }


        [HttpGet("{id}/histories")]
        public async Task<IActionResult> GetPagedHistoryAsync(int id, [FromQuery] RoleAccessPageRequest request)
        {
            var result = await _roleccessService.GePagedHistories(id, _mapper.Map<PagedDto>(request));
            if (result.Item1 == GetRoleAccessHistoiresEnum.RoleAccessNotFound)
                return NotFound(string.Format(NotExitsMessage, id));
            var response = GenerateNumber(_mapper.Map<Paged<RoleAccessHistoryResponse>>(result.Item2), request);
            return Ok(response);
        }
        [HttpGet("menu")]
        public async Task<IActionResult> GetMenu()
        {
            var result = await _roleccessService.GetMenu();
            return Ok(result);
        }
        private Paged<RoleAccessHistoryResponse> GenerateNumber(Paged<RoleAccessHistoryResponse> response, RoleAccessPageRequest request)
        {
            var index = 1;
            foreach (var item in response.Items)
            {
                item.No = index + ((request.Page - 1) * request.Size);
                index++;
            }
            return response;
        }
        [HttpPost("{id}/approval/{state}")]
        public async Task<IActionResult> ApprovalAsync(int id, [FromBody] ApprovalRequest? request, ActivityEnum state)
        {
            switch (state)
            {
                case ActivityEnum.Approved:
                    var result = await _roleccessService.ApprovedAsync(id, request.Note);
                    switch (result)
                    {
                        case ApprovedStateEnum.UserAccessNotFound:
                            return NotFound(string.Format(DataNotFound, id));
                        case ApprovedStateEnum.HasBeenProcced:
                            return NotFound(string.Format(Shared.Constants.ApiConstants.StatusApprovalMessage, ActivityEnum.Revised.ToString()));
                    }
                    Message = state == ActivityEnum.Approved ? ApiConstants.DataHasBeenApprovedMessage : ApiConstants.DataHasBeenRejectedMessage;
                    return Ok(true);
                case ActivityEnum.Revised:
                    var rejectedResult = await _roleccessService.RejectedAsync(id, request.Note);
                    switch (rejectedResult)
                    {
                        case RejectedStateEnum.UserAccessNotFound:
                            return NotFound(string.Format(DataNotFound, id));
                        case RejectedStateEnum.HasBeenProcced:
                            return NotFound(string.Format(Shared.Constants.ApiConstants.StatusApprovalMessage, ActivityEnum.Approved.ToString()));
                    }
                    Message = state == ActivityEnum.Approved ? ApiConstants.DataHasBeenApprovedMessage : ApiConstants.DataHasBeenRejectedMessage;
                    return Ok(true);

            }
            return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, [FromBody] BaseUserAccessRequest request)
        {
            var role = _mapper.Map<RoleAccessDto>(request);
            var result = await _roleccessService.UpdateAsync(id, role);
            switch (result.Item1)
            {
                case UpdateStateEnum.RoleNotFound:
                    return NotFound(string.Format(RoleConstants.NotExitsMessage, request.RoleId));
                case UpdateStateEnum.MenuNotFound:
                    return NotFound(string.Format(MenuConstants.DataNotFound));
                case UpdateStateEnum.WaitingApprovedByApproval:
                    return BadRequest(RoleAccessConstant.NeedProccedByApproval);
                case UpdateStateEnum.DuplicateRole:
                    return BadRequest(MenuConstants.RoleHasAnotherData);
                case UpdateStateEnum.UserAccessNotFound:
                    return NotFound(string.Format(DataNotFound, id));
            }
            Message = UpdateSuccessMessage;
            return Ok(_mapper.Map<RoleAccessResponse>(result.Item2));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            Message = DeleteFailedMessage;
            var result = await _roleccessService.DeleteAsync(id);
            switch (result)
            {
                case DeleteStateEnum.UserAccessNotFound:
                    return NotFound(ApiConstants.DeleteFailedMessage + " - " + string.Format(DataNotFound, id));
                case DeleteStateEnum.WaitingApprovedByApproval:
                    return BadRequest(ApiConstants.DeleteFailedMessage + " - " + RoleAccessConstant.NeedProccedByApproval);
            }
            Message = DeleteSuccessMessage;
            return Ok(true);
        }
        [HttpGet()]
        public async Task<IActionResult> GetPagedAsync([FromQuery] RoleAccessPageRequest request)
        {
            var items = await _roleccessService.GetPaged(_mapper.Map<PagedDto>(request), _mapper.Map<RoleAccessFilter>(request));
            return Ok(_mapper.Map<Paged<RoleAccessPagedResponse>>(items));
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetPreviewByIdAsync(int id)
        {
            var result = await _roleccessService.GetPreviewByIdAsync(id);
            switch (result.Item1)
            {
                case GetStateEnum.UserAccessNotFound:
                    return NotFound(string.Format(DataNotFound, id));
            }
            return Ok(_mapper.Map<RoleAccessResponse>(result.Item2));
        }


    }
}

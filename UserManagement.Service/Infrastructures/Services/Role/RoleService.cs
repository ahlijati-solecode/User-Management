using AutoMapper;
using Shared.Constants;
using Shared.Infrastructures.Repositories;
using Shared.Infrastructures.Repositories.TaskList;
using Shared.Infrastructures.Services.User;
using Shared.Models;
using Shared.Models.Core;
using Shared.Models.Dtos.Core;
using Shared.Models.Filters;
using User_Management.Infrastructures.Repositories;
using User_Management.Infrastructures.Repositories.RoleUser;
using User_Management.Infrastructures.Repositories.UserAccess;
using User_Management.Models.Dtos.Role;
using User_Management.Models.Entities;
using User_Management.Models.Filters;
using static Shared.Constants.ApiConstants;
using static User_Management.Constants.RoleConstants;

namespace User_Management.Infrastructures.Services.Role
{
    public class RoleService : IRoleService
    {
        private readonly IActivityRepository _activityRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ITaskRepository _taskRepository;
        private readonly IRoleAccessRefRepository _roleAccessRefRepository;
        private readonly IRoleUserRefRepository _roleUserRefRepository;
        private readonly IRoleHisotryRepository _roleHisotryRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWorks _unitOfWorks;

        public RoleService(IRoleRepository roleRepository,
            IRoleHisotryRepository roleHisotryRepository,
            IActivityRepository activityRepository,

            IUnitOfWorks unitOfWorks,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ITaskRepository taskRepository,
            IRoleAccessRefRepository roleAccessRefRepository,
            IRoleUserRefRepository roleUserRefRepository)
        {
            _roleRepository = roleRepository;
            _roleHisotryRepository = roleHisotryRepository;
            _activityRepository = activityRepository;
            _unitOfWorks = unitOfWorks;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _taskRepository = taskRepository;
            _roleAccessRefRepository = roleAccessRefRepository;
            _roleUserRefRepository = roleUserRefRepository;
        }

        public ActivityEnum CurrrentActivity { get; private set; }

        public async Task<Result<AddStateEnum, RoleDto?>> AddAsync(RoleDto model)
        {
            var role = _mapper.Map<MdRole>(model);
            if (await _roleRepository.IsDuplicate(model.Name, model.Id))
            {
                return new(AddStateEnum.DuplicateRoleName, null);
            }
            role = await AddHistoryAsync(role);
            _ = await _roleRepository.AddAsync(role);
            _ = await _activityRepository.AddAsync(string.Format(Shared.Constants.ApiConstants.CraetedMessage, TaskListEnum.Role, role.Name));
            await _unitOfWorks.SaveChangesAsync();
            await AddTaskList(role, ForInsertMessage);
            return new(AddStateEnum.Success, _mapper.Map<RoleDto>(role));
        }

        public async Task<DeleteStateEnum> DeleteAsync(int id)
        {
            var model = await _roleRepository.GetById(id);
            if (model == null) return DeleteStateEnum.NotFound;

            var roleAccess = await _roleAccessRefRepository.GetByRoles(new[] { id });
            var userRoles = await _roleUserRefRepository.GetByRoles(new[] { id });
            if (roleAccess != null && roleAccess.Any() && userRoles != null && userRoles.Any())
                return DeleteStateEnum.UsedInRoleAccessAndUserRole;
            if (roleAccess != null && roleAccess.Any())
                return DeleteStateEnum.UsedInRoleAccess;

            if (userRoles != null && userRoles.Any())
                return DeleteStateEnum.UsedInRoleUser;

            var latestLog = await _roleHisotryRepository.GetLatestLogByParent(model.Id);
            if (latestLog.Activity == ActivityEnum.Submitted.ToString())
                return DeleteStateEnum.WaitingApprovedByApproval;

            model = await AddHistoryAsync(model, _mapper.Map<RoleDto>(model), n =>
            {
                n.DeletedBy = _currentUserService.CurrentUser?.UserName;
                n.DeletedDate = DateTime.Now;
                n.User = _currentUserService.CurrentUser?.UserName;
            });



            await _roleRepository.UpdateAsync(model);
            await _activityRepository.AddAsync(string.Format(ApiConstants.DeletedMessage, TaskListEnum.Role.ToString(), model.Name));
            await _unitOfWorks.SaveChangesAsync();
            await AddTaskList(model, ForDeleteMessage);

            return DeleteStateEnum.Success;
        }


        public async Task<Result<GetByIdStateEnum, RoleDto>> GetAsync(int id)
        {
            var role = await _roleRepository.GetById(id, true);
            var model = await _roleRepository.GetById(id, true);
            if (model == null) return new(GetByIdStateEnum.RoleNotFound, null);
            var latestLog = await _roleHisotryRepository.GetLatestLogByParent(role.Id);
            latestLog.Id = id;
            return new(GetByIdStateEnum.Success, _mapper.Map<RoleDto>(latestLog));

        }

        public async Task<Paged<MdRolePagedDto>> GetPaged(PagedDto page, RoleFilter filter)
        {
            var items = await _roleRepository.GetPaged(filter, page.Page, page.Size, page.Sort);
            return _mapper.Map<Paged<MdRolePagedDto>>(items);
        }

        public async Task<ApprovalStateEnum> ApprovalAsync(int id, RoleDto model, ActivityEnum state)
        {
            var role = await _roleRepository.GetById(id, true);
            if (role == null)
                return ApprovalStateEnum.RoleNotFound;
            var statusApproval = await _roleHisotryRepository.CheckHasApproveOrReject(id);
            if (statusApproval.Item1)
            {
                CurrrentActivity = statusApproval.Item2;
                return ApprovalStateEnum.HasProcessed;
            }
            if (state == ActivityEnum.Approved)
            {
                var latestLog = await _roleHisotryRepository.GetLatestLogByParent(id);
                if (latestLog != null)
                {
                    role.ApprovedBy = _currentUserService.CurrentUser?.UserName;
                    role.ApprovedTime = DateTime.Now;
                    role.Name = latestLog.Name;
                    role.Description = latestLog.Description;
                    role.IsActive = latestLog.IsActive == null ? false : latestLog.IsActive.Value;
                    role.IsAdmin = latestLog.IsAdmin == null ? false : latestLog.IsAdmin.Value;
                    if (latestLog.DeletedBy != null)
                    {
                        role.DeletedBy = _currentUserService.CurrentUser?.UserName;
                        role.DeletedDate = DateTime.Now;
                    }
                }

                role.ApprovalStatus = true;
                if (role.IsAdmin)
                {
                    var access = role.MdUserAccesses.SelectMany(n => n.MdUserAccessRefs);
                    foreach (var item in access)
                    {
                        item.IsView = true;
                        await _roleAccessRefRepository.UpdateAsync(item);
                    }
                }
            }

            else if (state == ActivityEnum.Revised)
                role.ApprovalStatus = false;


            _ = await _roleRepository.UpdateAsync(role);
            _ = await _roleHisotryRepository.AddAsync(role, state, model.Note);
            await _taskRepository.DeleteByReferenceIdAsync(id, TaskListEnum.Role);
            await _activityRepository.AddAsync(string.Format(ApprovalMessage, role.Name, state.ToString()));
            await _unitOfWorks.SaveChangesAsync();
            return ApprovalStateEnum.Success;
        }

        public async Task<Result<UpdateStateEnum, RoleDto?>> UpdateAsync(int id, RoleDto model)
        {
            var role = await _roleRepository.GetById(id);
            if (role == null) return new(UpdateStateEnum.NotFound, null);
            if (await _roleRepository.IsDuplicate(model.Name, id))
            {
                return new(UpdateStateEnum.DuplicateRoleName, null);
            }
            var latestLog = await _roleHisotryRepository.GetLatestLogByParent(model.Id);

            if (latestLog.Activity == ActivityEnum.Submitted.ToString())
                return new(UpdateStateEnum.WaitingApprovedByApproval, null);
            role = await AddHistoryAsync(role, model);
            role.Id = id;
            role = await _roleRepository.UpdateAsync(role);
            await _unitOfWorks.SaveChangesAsync();
            await AddTaskList(role, ForUpdateMessage);
            await _activityRepository.AddAsync(string.Format(ApiConstants.UpdatedMessage, TaskListEnum.SystemVariable.ToString(), model.Name));

            return new(UpdateStateEnum.Success, _mapper.Map<RoleDto>(role));
        }

        private async Task AddTaskList(MdRole model, string messageInformation)
        {
            await _taskRepository.AddAsync(String.Format(TaskListMessage, "Role") + messageInformation, TaskListEnum.Role.ToString(), model.Id.ToString());
            await _unitOfWorks.SaveChangesAsync();
        }
        public async Task<MdRole> AddHistoryAsync(MdRole model, RoleDto input, Action<LgRole> action = null)
        {
            var log = _mapper.Map<LgRole>(input);
            log.Id = 0;
            log.User = _currentUserService.CurrentUser?.UserName;
            log.RoleId = model.Id;
            model.LgRoleHistories.Add(log);
            log.Activity = "Submitted";
            if (action != null)
                action.Invoke(log);
            _ = await _roleHisotryRepository.AddAsync(log);
            return model;
        }

        private async Task<MdRole> AddHistoryAsync(MdRole role)
        {
            var logRole = _mapper.Map<LgRole>(role);
            logRole.Activity = ActivityEnum.Submitted.ToString();
            logRole.User = _currentUserService.CurrentUser?.UserName;
            role.LgRoleHistories.Add(logRole);
            _ = await _roleHisotryRepository.AddAsync(logRole);
            return role;
        }

        public async Task<Result<GetRoleHistoiresEnum, Paged<LgRoleDto>?>> GePagedHistories(int roleId, PagedDto page)
        {
            var role = await _roleRepository.GetById(roleId);
            if (role == null)
                return new(GetRoleHistoiresEnum.RoleNotFound, null);
            return new(GetRoleHistoiresEnum.Success, _mapper.Map<Paged<LgRoleDto>>(await _roleHisotryRepository.GetPaged(roleId, _mapper.Map<PageFilter>(page))));
        }

        public Task<Paged<MdRolePagedDto>> GetActiveAsync(PagedDto pagedDto, RoleFilter roleFilter)
        {
            roleFilter.ApprovalStatus = "1";
            return GetPaged(pagedDto, roleFilter);
        }

        public async Task<RoleDto> GetActiveByNameAsync(string roleName)
        {
            return _mapper.Map<RoleDto>(await _roleRepository.GetActiveByNameAsync(roleName));
        }
    }
}
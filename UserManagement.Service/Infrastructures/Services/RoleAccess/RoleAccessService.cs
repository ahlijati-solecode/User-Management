using AutoMapper;
using Shared.Infrastructures.Repositories;
using Shared.Infrastructures.Repositories.TaskList;
using Shared.Infrastructures.Services.User;
using Shared.Models.Core;
using Shared.Models.Dtos.Core;
using Shared.Models.Filters;

using static Shared.Constants.ApiConstants;
using User_Management.Infrastructures.Services;
using User_Management.Infrastructures.Repositories;
using User_Management.Infrastructures.Repositories.UserAccess;
using static User_Management.Constants.RoleAccessConstant;
using User_Management.Models.Dtos.User;
using User_Management.Models.Entities;
using User_Management.Models.Filters;
using Shared.Models;

namespace Shared.Infrastructures.Services
{
    public class RoleAccessService : IRoleAccessService
    {
        private readonly IActivityRepository _activityRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly IRefMenuRepository _refMenuRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWorks _unitOfWorks;
        private readonly IRoleAccessLogRepository _roleAccessLogRepository;
        private readonly IRoleAccessRefLogRepository _roleAccessRefLogRepository;
        private readonly IRoleAccessRefRepository _roleAccessRefRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IRoleAccessRepository _roleAccessRepository;
        public RoleAccessService(IMapper mapper,
            ICurrentUserService currentUserService,
            IRoleAccessRepository roleAccessRepository,
            IUnitOfWorks unitOfWorks,
            IActivityRepository activityRepository,
            IRoleRepository roleRepository,
            IRefMenuRepository refMenuRepository,
            IRoleAccessLogRepository roleAccessLogRepository,
            IRoleAccessRefLogRepository roleAccessRefLogRepository,
            IRoleAccessRefRepository roleAccessRefRepository,
            ITaskRepository taskRepository)
        {
            _mapper = mapper;
            _currentUserService = currentUserService;
            _roleAccessRepository = roleAccessRepository;
            _unitOfWorks = unitOfWorks;
            _activityRepository = activityRepository;
            _roleRepository = roleRepository;
            _refMenuRepository = refMenuRepository;
            _roleAccessLogRepository = roleAccessLogRepository;
            _roleAccessRefLogRepository = roleAccessRefLogRepository;
            _roleAccessRefRepository = roleAccessRefRepository;
            _taskRepository = taskRepository;
        }
        public async Task<Tuple<AddStateEnum, RoleAccessDto>> AddAsync(RoleAccessDto input)
        {
            var roleAccess = _mapper.Map<MdRoleAccess>(input);
            var role = await _roleRepository.GetById(roleAccess.RoleId);
            if (role == null)
                return new(AddStateEnum.RoleNotFound, null);
            if (await _roleAccessRepository.IsDuplicateAsync(input.RoleId, 0))
                return new(AddStateEnum.DuplicateRole, new RoleAccessDto() { Name = (await _roleRepository.GetById(input.RoleId))?.Name });

            var logUserAccess = _mapper.Map<LgRoleAccess>(roleAccess);
            logUserAccess.Activity = ActivityEnum.Submitted.ToString();
            await _roleAccessLogRepository.AddAsync(logUserAccess);
            roleAccess.LgUserAccesses.Add(logUserAccess);
            var roleAccessReference = _mapper.Map<IEnumerable<MdRoleAccessRef>>(input.MdUserAccessRefs).ToList();
            foreach (var item in roleAccessReference)
            {
                var menu = await _refMenuRepository.GetById(item.RefMenuId);
                if (menu == null)
                {
                    return new(AddStateEnum.MenuNotFound, null);
                }
                item.CreatedBy = logUserAccess.CreatedBy;
                item.CreatedDate = DateTime.Now;
                var logRef = _mapper.Map<LgRoleAccessRef>(item);
                logRef.RefUserAccessNavigation = roleAccess;
                item.LgUserAccessRefs.Add(logRef);
                await _roleAccessRefLogRepository.AddAsync(logRef);
                ResetAllPermisionBeforeApproved(item);
                if (role.IsAdmin)
                {
                    SetAsAdmin(role, item, logRef);
                }
            }
            await SetPermissionForAdmin(roleAccessReference, roleAccess, role);

            ResetAllAccessBeforeApproved(roleAccess);
            roleAccess.MdUserAccessRefs = roleAccessReference;
            await _roleAccessRepository.AddAsync(roleAccess);

            _ = await _activityRepository.AddAsync(string.Format(Constants.ApiConstants.CraetedMessage, TaskListEnum.RoleUser.ToString(), role?.Name));
            await _unitOfWorks.SaveChangesAsync();
            await _taskRepository.AddAsync(String.Format(TaskListMessage, "Role Access") + ForInsertMessage, TaskListEnum.RoleAccess.ToString(), roleAccess.Id.ToString());
            await _unitOfWorks.SaveChangesAsync();
            return new(AddStateEnum.Success, _mapper.Map<RoleAccessDto>(roleAccess));
        }

        private static void SetAsAdmin(MdRole role, MdRoleAccessRef reference, LgRoleAccessRef logRef)
        {
            reference.IsView = role.IsAdmin;
            logRef.IsView = role.IsAdmin;
        }

        public async Task<ApprovedStateEnum> ApprovedAsync(int id, string note)
        {
            var roleAccess = await _roleAccessRepository.GetById(id);
            if (roleAccess == null)
                return ApprovedStateEnum.UserAccessNotFound;
            var logUserAccess = await _roleAccessLogRepository.GetLatestByUserAccessId(roleAccess.Id);
            if (logUserAccess != null)
            {
                if (logUserAccess.Activity != ActivityEnum.Submitted.ToString())
                    return ApprovedStateEnum.HasBeenProcced;
                logUserAccess.Note = note;
                logUserAccess.Activity = ActivityEnum.Approved.ToString();
                _ = await _activityRepository.AddAsync(string.Format(ApprovedMessage, roleAccess.Role?.Name));

                MigrateDataToMaster(roleAccess, logUserAccess);
                roleAccess.ApprovedDate = DateTime.Now;
                roleAccess.ApprovedBy = _currentUserService.CurrentUser?.UserName;

                if (roleAccess.DeletedBy != null)
                {
                    await MarkDeleted(roleAccess);
                }
                else
                {
                    await ApprovedThePermission(roleAccess);
                }
                await AddHistoryAsync(roleAccess, ActivityEnum.Approved, note);
                await _roleAccessRepository.UpdateAsync(roleAccess);
                await _taskRepository.DeleteByReferenceIdAsync(id, TaskListEnum.RoleAccess);
                await _unitOfWorks.SaveChangesAsync();
            }
            return ApprovedStateEnum.Success;
        }

        private async Task AddHistoryAsync(MdRoleAccess roleAccess, ActivityEnum state, string note)
        {
            var latestLog = _mapper.Map<LgRoleAccess>(roleAccess);
            latestLog.Id = 0;
            latestLog.ParentId = roleAccess.Id;
            latestLog.Activity = state.ToString();
            latestLog.Note = note;
            foreach (var reference in roleAccess.MdUserAccessRefs)
            {
                var latestRefLog = _mapper.Map<LgRoleAccessRef>(reference);
                latestRefLog.Id = 0;
                latestRefLog.ParentId = reference.Id;
                latestRefLog.RefUserAccessNavigation = null;
                await _roleAccessRefLogRepository.AddAsync(latestRefLog);
            }
            await _roleAccessLogRepository.AddAsync(latestLog);
        }

        private async Task ApprovedThePermission(MdRoleAccess roleAccess)
        {
            foreach (var reference in roleAccess.MdUserAccessRefs)
            {
                var item = reference;
                var logReference = await _roleAccessRefLogRepository.GetLatestLog(reference.Id);
                if (logReference != null)
                {
                    item = AssignPermission(item, logReference);

                }
                await _roleAccessRefRepository.UpdateAsync(item);
            }
        }

        private async Task MarkDeleted(MdRoleAccess roleAccess)
        {
            foreach (var item in await _roleAccessLogRepository.GetLogs(roleAccess.Id))
            {
                await _roleAccessLogRepository.DeleteAsync(item);
            }
            foreach (var item in roleAccess.LgUserAccessRefs)
            {
                await _roleAccessRefLogRepository.DeleteAsync(item);
            }
            foreach (var reference in roleAccess.MdUserAccessRefs)
            {
                var item = reference;
                await _roleAccessRefRepository.DeleteAsync(item);
            }
        }

        private static void MigrateDataToMaster(MdRoleAccess roleAccess, LgRoleAccess logUserAccess)
        {
            roleAccess.IsActive = logUserAccess.IsActive;
            roleAccess.RoleId = logUserAccess.RoleId;
            roleAccess.DeletedBy = logUserAccess.DeletedBy;
            roleAccess.DeletedDate = logUserAccess.DeletedDate;
        }

        public async Task<DeleteStateEnum> DeleteAsync(int id)
        {
            var roleAccess = await _roleAccessRepository.GetById(id);
            if (roleAccess == null)
                return DeleteStateEnum.UserAccessNotFound;
            LgRoleAccess logUserAccess = await _roleAccessLogRepository.GetLatestByUserAccessId(id);
            if (logUserAccess?.Activity == ActivityEnum.Submitted.ToString())
                return DeleteStateEnum.WaitingApprovedByApproval;
            var logAccessRole = await CreateNewLogger(roleAccess);
            logAccessRole.DeletedBy = _currentUserService.CurrentUser?.UserName;
            logAccessRole.DeletedDate = DateTime.Now;
            foreach (var item in roleAccess.MdUserAccessRefs)
            {
                var log = _mapper.Map<LgRoleAccessRef>(item);
                log.Id = 0;
                log.ParentId = item.Id;
                log.RefUserAccess = roleAccess.Id;
                log.RefUserAccessNavigation = null;
                log.DeletedBy = logAccessRole.DeletedBy;
                log.DeletedDate = logAccessRole.DeletedDate;
                await _roleAccessRefLogRepository.AddAsync(log);
            }
            await _roleAccessLogRepository.AddAsync(logAccessRole);


            await _taskRepository.AddAsync(String.Format(TaskListMessage, "Role Access") + ForDeleteMessage, TaskListEnum.RoleAccess.ToString(), id.ToString());
            await _unitOfWorks.SaveChangesAsync();
            return DeleteStateEnum.Success;
        }

        private async Task<LgRoleAccess> CreateNewLogger(MdRoleAccess roleAccess)
        {
            LgRoleAccess logUserAccess = _mapper.Map<LgRoleAccess>(roleAccess);
            logUserAccess.Id = 0;
            logUserAccess.ParentId = roleAccess.Id;

            logUserAccess.Activity = ActivityEnum.Submitted.ToString();

            await _roleAccessLogRepository.AddAsync(logUserAccess);


            return logUserAccess;
        }

        public async Task<Tuple<GetRoleAccessHistoiresEnum, Paged<RoleAccessHistoryDto>?>> GePagedHistories(int id, PagedDto pagedDto)
        {
            var roleAccess = await _roleAccessRepository.GetById(id);
            if (roleAccess == null)
                return new(GetRoleAccessHistoiresEnum.RoleAccessNotFound, null);
            return new(GetRoleAccessHistoiresEnum.Success, _mapper.Map<Paged<RoleAccessHistoryDto>>(await _roleAccessLogRepository.GetPaged(id, _mapper.Map<PageFilter>(pagedDto))));
        }

        public async Task<Paged<RoleAccessDto>> GetPaged(PagedDto pagedDto, RoleAccessFilter userAccessFilter)
        {
            return _mapper.Map<Paged<RoleAccessDto>>(await _roleAccessRepository.GetPaged(userAccessFilter, pagedDto.Page, pagedDto.Size, pagedDto.Sort));
        }

        public async Task<Tuple<GetStateEnum, RoleAccessDto>> GetPreviewByIdAsync(int id)
        {
            var roleAccess = await _roleAccessRepository.GetPreviewByIdAsync(id);
            if (roleAccess == null)
                return new(GetStateEnum.UserAccessNotFound, null);
            var output = _mapper.Map<RoleAccessDto>(roleAccess);
            foreach (var item in await _refMenuRepository.GetAllMenuData())
            {
                if (!output.MdUserAccessRefs.Any(m => m.RefMenuId == item.Id))
                {
                    output.MdUserAccessRefs.Add(CreateDefaultMenu(item));
                }
            }
            return new(GetStateEnum.Success, output);
        }

        private static MdRoleAccessRefDto CreateDefaultMenu(MdRefMenu item)
        {
            return new MdRoleAccessRefDto()
            {
                RefMenuId = item.Id,
                IsCreate = false,
                IsDelete = false,
                IsEdit = false,
                IsView = false
            };
        }

        public async Task<RejectedStateEnum> RejectedAsync(int id, string note)
        {
            var roleAccess = await _roleAccessRepository.GetById(id);
            if (roleAccess == null)
                return RejectedStateEnum.UserAccessNotFound;
            var logUserAccess = await _roleAccessLogRepository.GetLatestByUserAccessId(roleAccess.Id);
            if (logUserAccess != null)
            {
                if (logUserAccess.Activity != ActivityEnum.Submitted.ToString())
                    return RejectedStateEnum.HasBeenProcced;
                logUserAccess.Note = note;
                logUserAccess.Activity = ActivityEnum.Revised.ToString();
                logUserAccess.IsActive = roleAccess.IsActive;
                await AddHistoryAsync(roleAccess, ActivityEnum.Revised, note);
                _ = await _activityRepository.AddAsync(string.Format(RejectedMessage, roleAccess.Role?.Name));
                await _taskRepository.DeleteByReferenceIdAsync(id, TaskListEnum.RoleAccess);
                await _unitOfWorks.SaveChangesAsync();
            }
            return RejectedStateEnum.Success;
        }

        public async Task<Tuple<UpdateStateEnum, RoleAccessDto>> UpdateAsync(int id, RoleAccessDto input)
        {
            var roleAccess = await _roleAccessRepository.GetById(id);
            if (roleAccess == null)
                return new(UpdateStateEnum.UserAccessNotFound, null);

            if (await _roleAccessRepository.IsDuplicateAsync(input.RoleId, id))
                return new(UpdateStateEnum.DuplicateRole, null);
            var role = await _roleRepository.GetById(roleAccess.RoleId);
            if (role == null)
                return new(UpdateStateEnum.RoleNotFound, null);
            var isNewSubmited = false;
            LgRoleAccess logUserAccess = await _roleAccessLogRepository.GetLatestByUserAccessId(id);
            if (logUserAccess != null)
            {
                if (logUserAccess.Activity != ActivityEnum.Submitted.ToString())
                {
                    var temp = roleAccess.IsActive;
                    roleAccess.IsActive = input.IsActive;
                    await CreateNewLogger(roleAccess);
                    roleAccess.IsActive = temp;
                    isNewSubmited = true;
                }
                else
                {
                    return new(UpdateStateEnum.WaitingApprovedByApproval, null);
                }
            }

            var userAccessReference = _mapper.Map<IEnumerable<MdRoleAccessRef>>(input.MdUserAccessRefs).ToList();
            var exisstingReference = roleAccess.MdUserAccessRefs.Where(n => n.DeletedBy is null);
            var result = await AssignAccessPermission(userAccessReference, exisstingReference, isNewSubmited, roleAccess, role, logUserAccess);
            if (result.State != UpdateStateEnum.Success)
            {
                return new(UpdateStateEnum.MenuNotFound, null);
            }
            await _roleAccessRepository.UpdateAsync(roleAccess);
            await _taskRepository.AddAsync(String.Format(TaskListMessage, "Role Access") + ForUpdateMessage, TaskListEnum.RoleAccess.ToString(), id.ToString());
            _ = await _activityRepository.AddAsync(string.Format(Shared.Constants.ApiConstants.UpdatedMessage, TaskListEnum.RoleUser.ToString(), role.Name));
            await _unitOfWorks.SaveChangesAsync();

            return new(UpdateStateEnum.Success, _mapper.Map<RoleAccessDto>(roleAccess));
        }

        private async Task<Result<UpdateStateEnum, string>> AssignAccessPermission(IEnumerable<MdRoleAccessRef> userAccessReference,
            IEnumerable<MdRoleAccessRef> exisstingReference, bool isNewSubmited, MdRoleAccess roleAccess, MdRole role, LgRoleAccess? logUserAccess)
        {
            foreach (var item in userAccessReference)
            {
                LgRoleAccessRef logRef;
                var menu = await _refMenuRepository.GetById(item.RefMenuId);
                if (menu == null)
                {
                    return new(UpdateStateEnum.MenuNotFound, null);
                }
                var existing = exisstingReference.FirstOrDefault(n => n.RefMenuId == menu.Id);
                if (isNewSubmited)
                {

                    logRef = _mapper.Map<LgRoleAccessRef>(item);
                    logRef.RefUserAccessNavigation = roleAccess;
                    if (existing != null)
                    {
                        logRef.ParentId = existing.Id;
                        logRef.RefUserAccess = roleAccess.Id;
                        logRef.RefUserAccessNavigation = null;

                        await _roleAccessRefLogRepository.AddAsync(logRef);
                    }
                    else
                    {
                        logRef = await CreateNewRoleAccess(roleAccess, item, logUserAccess);
                    }
                }
                else if (existing == null)
                {
                    logRef = await CreateNewRoleAccess(roleAccess, item);
                }
                else
                {
                    logRef = await _roleAccessRefLogRepository.GetLatestLog(existing.Id);
                    logRef = AssignPermission(logRef, _mapper.Map<LgRoleAccessRef>(item));
                    await _roleAccessRefLogRepository.UpdateAsync(logRef);
                }
                if (role.IsAdmin)
                {
                    SetAsAdmin(role, item, logRef);
                }

            }
            await SetPermissionForAdmin(userAccessReference, roleAccess, role);

            return new(UpdateStateEnum.Success, null);

        }

        private async Task SetPermissionForAdmin(IEnumerable<MdRoleAccessRef> userAccessReference, MdRoleAccess roleAccess, MdRole role)
        {
            if (role.IsAdmin)
            {
                var menus = await _refMenuRepository.GetAllMenuData();
                foreach (var item in menus.Where(x => !userAccessReference.Any(m => m.RefMenuId == x.Id)))
                {
                    var menuRef = new MdRoleAccessRef();
                    menuRef.RefMenuId = item.Id;

                    menuRef.IsView = true;
                    if (roleAccess.Id > 0)
                        menuRef.RefUserAccess = roleAccess.Id;
                    else
                    {
                        menuRef.RefUserAccessNavigation = roleAccess;
                        roleAccess.MdUserAccessRefs.Add(menuRef);
                    }
                    var logRef = _mapper.Map<LgRoleAccessRef>(menuRef);
                    menuRef.LgUserAccessRefs.Add(logRef);
                    await _roleAccessRefLogRepository.AddAsync(logRef);

                    await _roleAccessRefRepository.AddAsync(menuRef);
                }
            }
        }

        private async Task<LgRoleAccessRef> CreateNewRoleAccess(MdRoleAccess roleAccess, MdRoleAccessRef item, LgRoleAccess? logUserAccess = null)
        {
            item.CreatedBy = _currentUserService.CurrentUser?.UserName;
            item.CreatedDate = DateTime.Now;
            var logRef = _mapper.Map<LgRoleAccessRef>(item);
            logRef.Id = 0;
            logRef.RefUserAccess = roleAccess.Id;
            logRef.RefUserAccessNavigation = null;
            if (logUserAccess != null)
                logRef.Parent = item;
            await _roleAccessRefLogRepository.AddAsync(logRef);
            ResetAllPermisionBeforeApproved(item);
            roleAccess.MdUserAccessRefs.Add(item);
            return logRef;
        }

        private LgRoleAccessRef AssignPermission(LgRoleAccessRef existing, LgRoleAccessRef item)
        {
            existing.IsCreate = item.IsCreate;
            existing.IsDelete = item.IsDelete;
            existing.IsEdit = item.IsEdit;
            existing.IsView = item.IsView;
            return existing;
        }

        private MdRoleAccessRef AssignPermission(MdRoleAccessRef existing, LgRoleAccessRef item)
        {
            existing.IsCreate = item.IsCreate;
            existing.IsDelete = item.IsDelete;
            existing.IsEdit = item.IsEdit;
            existing.IsView = item.IsView;
            existing.RefMenuId = item.RefMenuId;

            existing.DeletedBy = item.DeletedBy;
            existing.DeletedDate = item.DeletedDate;

            return existing;
        }
        private void ResetAllAccessBeforeApproved(MdRoleAccess roleAccess)
        {
            roleAccess.IsActive = false;
        }

        private void ResetAllPermisionBeforeApproved(MdRoleAccessRef item)
        {
            item.IsCreate = false;
            item.IsDelete = false;
            item.IsEdit = false;
            item.IsView = false;
        }

        public async Task<IEnumerable<MenuDto>> GetMenu()
        {
            return _mapper.Map<IEnumerable<MenuDto>>(await _refMenuRepository.GetAllMenuData());
        }
    }
}
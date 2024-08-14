using AutoMapper;
using User_Management.Infrastructures.Repositories;
using User_Management.Models.Filters;
using User_Management.Infrastructures.Repositories.RoleUser;
using User_Management.Models.Entities;
using static User_Management.Constants.RoleUserConstants;
using User_Management.Models.Dtos.UserRole;
using User_Management.Models.Dtos.Role;
using Shared.Infrastructures.Repositories;
using Shared.Infrastructures.Repositories.Configuration;
using Shared.Infrastructures.Services.User;
using Shared.Infrastructures.Repositories.TaskList;
using Shared.Models;
using static Shared.Constants.ApiConstants;
using Shared.Models.Entities.Custom.Configuration;
using Shared.Models.Core;
using Shared.Models.Dtos.Core;
using Shared.Models.Filters;

namespace User_Management.Infrastructures.Services
{
    public class RoleUserService : IRoleUserService
    {
        private readonly ILogRoleUserRepository _logRoleUserRepository;
        private readonly ILogRoleUserRefRepository _logRoleUserRefRepository;
        private readonly IRoleUserRefRepository _roleUserRefRepository;
        private readonly IRoleUserRepository _roleUserRepository;
        private readonly ITemporaryRoleUserRefRepository _temporaryRoleUserRefRepository;
        private readonly ITemporaryRoleUserRepository _temporaryRoleUserRepository;
        private readonly IUnitOfWorks _unitOfWorks;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly ISearchUserRepository _searchUserRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IActivityRepository _activityRepository;
        private readonly ITaskRepository _taskRepository;

        public RoleUserService(ILogRoleUserRepository logRoleUserRepository,
            ILogRoleUserRefRepository logRoleUserRefRepository,
            IRoleUserRefRepository roleUserRefRepository,
            IRoleUserRepository roleUserRepository,
            ITemporaryRoleUserRefRepository temporaryRoleUserRefRepository,
            ITemporaryRoleUserRepository temporaryRoleUserRepository,
            IUnitOfWorks unitOfWorks,
            IRoleRepository roleRepository,
            IMapper mapper,
            ISearchUserRepository searchUserRepository,
            IHttpContextAccessor httpContextAccessor,
            IConfigurationRepository configurationRepository,
            ICurrentUserService currentUserService,
            IActivityRepository activityRepository,
            ITaskRepository taskRepository)
        {
            _logRoleUserRepository = logRoleUserRepository;
            _logRoleUserRefRepository = logRoleUserRefRepository;
            _roleUserRefRepository = roleUserRefRepository;
            _roleUserRepository = roleUserRepository;
            _temporaryRoleUserRefRepository = temporaryRoleUserRefRepository;
            _temporaryRoleUserRepository = temporaryRoleUserRepository;
            _unitOfWorks = unitOfWorks;
            _roleRepository = roleRepository;
            _mapper = mapper;
            _searchUserRepository = searchUserRepository;
            _httpContextAccessor = httpContextAccessor;
            _configurationRepository = configurationRepository;
            _currentUserService = currentUserService;
            _activityRepository = activityRepository;
            _taskRepository = taskRepository;
        }

        public async Task<Result<AddRoleUserTemporaryStatEnum, string>> AddRoleUserAsync(string uniqueId, RoleUserDto roleUserDto)
        {
            await InitilizeConfiguration();
            var temporaryRoleUser = await _temporaryRoleUserRepository.GetById(uniqueId);
            if (temporaryRoleUser == null)
                return new(AddRoleUserTemporaryStatEnum.ParentNotFound, null);
            var reference = _mapper.Map<TmpRoleUserRef>(roleUserDto);
            var role = await _roleRepository.GetActiveById(roleUserDto.RoleId);
            if (role == null)
                return new(AddRoleUserTemporaryStatEnum.RoleNotFound, null);
            if (await _roleUserRepository.IsDuplicate(roleUserDto.RoleId, temporaryRoleUser.ParentId ?? 0))
            {
                return new(AddRoleUserTemporaryStatEnum.DuplicateRole, role?.Name);
            }
            var user = await _searchUserRepository.GetById(roleUserDto.Username);
            if (user == null)
            {
                return new(AddRoleUserTemporaryStatEnum.UserNotFound, null);
            }
            var item = await _temporaryRoleUserRefRepository.IsDuplicate(uniqueId, roleUserDto.Username);
            if (item)
                return new(AddRoleUserTemporaryStatEnum.DuplicateUser, null);
            AssignReference(uniqueId, temporaryRoleUser, reference, user);

            temporaryRoleUser.RoleId = roleUserDto.RoleId;
            temporaryRoleUser.TmpRoleUserRefs.Add(reference);
            await _temporaryRoleUserRefRepository.AddAsync(reference);
            await _temporaryRoleUserRepository.UpdateAsync(temporaryRoleUser);
            await _unitOfWorks.SaveChangesAsync();


            return new(AddRoleUserTemporaryStatEnum.Success, reference.Id);
        }

        private static void AssignReference(string uniqueId, TmpRoleUser temporaryRoleUser, TmpRoleUserRef reference, Shared.Models.Core.User? user)
        {
            reference.ParentId = uniqueId;
            reference.Username = user?.UserName;
            reference.Departement = user?.Departement;
            reference.FullName = user?.FullName;
            reference.Email = user.Email;
            reference.ParentId = temporaryRoleUser.Id;
        }

        public async Task<Result<CreateUniqueStateEnum, string>> CreateUniqueAsync(int? roleUserId, bool justView = false)
        {
            var newId = Guid.NewGuid().ToString();
            if (roleUserId > 0)
            {
                var roleUser = await _roleUserRepository.GetById(roleUserId.Value);
                if (roleUser == null)
                    return new(CreateUniqueStateEnum.RoleUserNotFound, null);

                var logUserAccess = await _logRoleUserRepository.GetLatestLogByParentId(roleUser.Id);
                if (justView != true)
                {
                    if (logUserAccess.Activity == ActivityEnum.Submitted.ToString())
                        return new(CreateUniqueStateEnum.WaitingApprovedByApproval, string.Empty);
                }

                var temp = _mapper.Map<TmpRoleUser>(logUserAccess);
                temp.Id = newId;
                temp.ParentId = roleUser.Id;
                foreach (var item in temp.TmpRoleUserRefs)
                {
                    //item.RefParentEmail = newId;
                    var childId = Guid.NewGuid().ToString();
                    item.ParentId = newId;
                    item.Id = childId;
                    await _temporaryRoleUserRefRepository.AddAsync(item);
                }
                await _temporaryRoleUserRepository.AddAsync(temp);
            }
            else
            {
                await _temporaryRoleUserRepository.AddAsync(new Models.Entities.TmpRoleUser
                {
                    Id = newId,
                    IsActive = true,

                });
            }

            await _unitOfWorks.SaveChangesAsync();
            _unitOfWorks.ChangeTrackerClear();
            return new(CreateUniqueStateEnum.Success, newId);
        }

        public async Task<Result<UpdateRoleUserTemporaryStatEnum, string>> UpdateRoleUserAsync(string uniqueId, string id, RoleUserDto roleUserDto)
        {
            await InitilizeConfiguration();

            var temporaryRoleUser = await _temporaryRoleUserRepository.GetById(uniqueId);
            if (temporaryRoleUser == null)
                return new(UpdateRoleUserTemporaryStatEnum.ParentNotFound, null);

            var role = await _roleRepository.GetActiveById(roleUserDto.RoleId);
            if (role == null)
                return new(UpdateRoleUserTemporaryStatEnum.RoleNotFound, null);
            var reference = await _temporaryRoleUserRefRepository.GetById(id);
            if (reference == null)
                return new(UpdateRoleUserTemporaryStatEnum.ItemNotFound, null);
            var user = await _searchUserRepository.GetById(roleUserDto.Username);
            if (user == null)
            {
                return new(UpdateRoleUserTemporaryStatEnum.UserNotFound, null);
            }
            var item = await _temporaryRoleUserRefRepository.GetPaged(uniqueId, _mapper.Map<PageFilter>(new PagedDto()), new RoleUserFilter() { Name = roleUserDto.Username });
            if (item.Items.Any(n => n.Id != id))
                return new(UpdateRoleUserTemporaryStatEnum.DuplicateUser, null);
            reference = _mapper.Map<TmpRoleUserRef>(roleUserDto);

            AssignReference(uniqueId, temporaryRoleUser, reference, user);
            reference.Id = id;
            temporaryRoleUser.RoleId = roleUserDto.RoleId;
            reference.ParentId = temporaryRoleUser.Id;
            await _temporaryRoleUserRefRepository.UpdateAsync(reference);
            await _unitOfWorks.SaveChangesAsync();


            return new(UpdateRoleUserTemporaryStatEnum.Success, reference.Id);
        }

        private async Task<ActiveDirectory> InitilizeConfiguration()
        {
            var configuration = await _configurationRepository.GetActiveDirectory();
            if (configuration != null)
            {
                if (_httpContextAccessor.HttpContext != null && !_httpContextAccessor.HttpContext.Items.ContainsKey(ActivityDirectoryConfiguration))
                    _httpContextAccessor.HttpContext.Items.Add(ActivityDirectoryConfiguration, configuration);
            }

            return configuration;
        }

        public async Task<Result<DeletedRoleUserTemporaryStatEnum, string>> DeleteRoleUserAsync(string uniqueId, string id)
        {
            var temporaryRoleUser = await _temporaryRoleUserRepository.GetById(uniqueId);
            if (temporaryRoleUser == null)
                return new(DeletedRoleUserTemporaryStatEnum.ParentNotFound, null);
            var reference = await _temporaryRoleUserRefRepository.GetById(id);
            if (reference == null)
                return new(DeletedRoleUserTemporaryStatEnum.ItemNotFound, null);
            await _temporaryRoleUserRefRepository.DeleteAsync(reference, true);
            await _unitOfWorks.SaveChangesAsync();
            return new(DeletedRoleUserTemporaryStatEnum.Success, reference.Id);
        }

        public async Task<Paged<TmpRoleUserRefDto>> GetPagedRoleUserAsync(string uniqueId, PagedDto pagedDto, RoleUserFilter RoleUserFilter)
        {
            var result = await _temporaryRoleUserRefRepository.GetPaged(uniqueId, _mapper.Map<PageFilter>(pagedDto), RoleUserFilter);
            return _mapper.Map<Paged<TmpRoleUserRefDto>>(result);
        }

        public async Task<Result<SaveStatEnum, int?>> CommitRoleUserAsync(string uniqueId, RoleUserDto input)
        {
            var temporaryRoleUser = await _temporaryRoleUserRepository.GetById(uniqueId);
            if (temporaryRoleUser == null)
                return new(SaveStatEnum.ParentNotFound, null);
            MdRoleUser roleUser;
            bool isNew = true;
            temporaryRoleUser.Role = null;
            if (!(input.Id > 0))
            {

                roleUser = _mapper.Map<MdRoleUser>(temporaryRoleUser);
                foreach (var item in temporaryRoleUser.TmpRoleUserRefs)
                {
                    var user = _mapper.Map<MdRoleUserRef>(item);
                    roleUser.MdRoleUserRefs.Add(user);
                    await _roleUserRefRepository.AddAsync(user);
                }
                await _roleUserRepository.AddAsync(roleUser);
            }
            else
            {
                isNew = false;
                roleUser = await _roleUserRepository.GetById(input.Id.Value);
                if (roleUser == null)
                    return new(SaveStatEnum.ItemNotFound, null);
                await _roleUserRepository.UpdateAsync(roleUser);
            }
            temporaryRoleUser.IsActive = input.IsActive;
            await AddNewLogs(temporaryRoleUser, roleUser);

            await _unitOfWorks.SaveChangesAsync();
            var message = (isNew ? ForInsertMessage : ForUpdateMessage);
            await _activityRepository.AddAsync($"{message} User Role ({(await GetRole(roleUser.RoleId))?.Name})");
            await _taskRepository.AddAsync(String.Format(TaskListMessage, "UserRole") + message, TaskListEnum.RoleUser.ToString(), roleUser.Id.ToString());
            await _unitOfWorks.SaveChangesAsync();
            _unitOfWorks.ChangeTrackerClear();
            return new(SaveStatEnum.Success, roleUser.Id);
        }

        private async Task AddNewLogs(TmpRoleUser temporaryRoleUser, MdRoleUser roleUser)
        {
            var logRoleUser = _mapper.Map<LgRoleUser>(roleUser);
            logRoleUser.Parent = roleUser;
            logRoleUser.Activity = ActivityEnum.Submitted.ToString();
            logRoleUser.IsActive = temporaryRoleUser.IsActive;
            foreach (var item in temporaryRoleUser.TmpRoleUserRefs)
            {
                var user = _mapper.Map<LgRoleUserRef>(item);
                user.Parent = logRoleUser;
                logRoleUser.LgRoleUserRefs.Add(user);
                await _logRoleUserRefRepository.AddAsync(user);
            }
            await _logRoleUserRepository.AddAsync(logRoleUser);
            roleUser.LgRoleUsers.Add(logRoleUser);
        }

        public async Task<Paged<LgRoleAccessDto>> GetPaged(PagedDto pagedDto, RoleUserFilter roleUserFilter)
        {
            var result = await _roleUserRepository.GetPaged(roleUserFilter, pagedDto.Page, pagedDto.Size, pagedDto.Sort);
            foreach (var item in result.Items)
            {
                var users = await _logRoleUserRefRepository.GetUsers(item.LogId, 5);
                item.Users = users.Select(m => _mapper.Map<User>(m)).ToList();
            }
            return _mapper.Map<Paged<LgRoleAccessDto>>(result);
        }

        public async Task<Result<DeleteStateEnum, int?>> DeleteUserAsync(int id)
        {
            var roleUser = await _roleUserRepository.GetById(id);
            if (roleUser == null)
                return new(DeleteStateEnum.ItemNotFound, null);
            var logUserAccess = await _logRoleUserRepository.GetLatestLogByParentId(roleUser.Id);

            if (logUserAccess.Activity == ActivityEnum.Submitted.ToString())
                return new(DeleteStateEnum.WaitingApprovedByApproval, null);

            var lgEmail = _mapper.Map<LgRoleUser>(roleUser);
            lgEmail.Activity = ActivityEnum.Submitted.ToString();

            AssignDeleteData(lgEmail, roleUser);
            AddReferenceUser(lgEmail, logUserAccess);
            await _logRoleUserRepository.AddAsync(lgEmail);
            await _unitOfWorks.SaveChangesAsync();

            var message = ForDeleteMessage;
            await _activityRepository.AddAsync($"{message} User Role ({(await GetRole(roleUser.RoleId))?.Name})");
            await _taskRepository.AddAsync(String.Format(TaskListMessage, "UserRole") + message, TaskListEnum.RoleUser.ToString(), roleUser.Id.ToString());
            await _unitOfWorks.SaveChangesAsync();
            return new(DeleteStateEnum.Success, roleUser.Id);
        }

        private void AddReferenceUser(LgRoleUser lgRoleUser, LgRoleUser logUserAccess)
        {
            foreach (var item in logUserAccess.LgRoleUserRefs)
            {
                var newItem = (LgRoleUserRef)item.Clone();
                newItem.Id = 0;
                lgRoleUser.LgRoleUserRefs.Add(newItem);
            }

        }

        private Task<MdRole> GetRole(int roleId)
        {
            return _roleRepository.GetById(roleId);
        }

        private void AssignDeleteData(LgRoleUser logRoleUser, MdRoleUser roleUser)
        {
            logRoleUser.DeletedBy = _currentUserService?.CurrentUser?.UserName;
            logRoleUser.DeletedDate = DateTime.Now;
            logRoleUser.ParentId = roleUser.Id;

            logRoleUser.Id = 0;
        }

        public async Task<Result<ApprovedStateEnum, int?>> ApprovedAsync(int id, ActivityEnum state, string note)
        {
            var roleUser = await _roleUserRepository.GetById(id);
            if (roleUser == null)
                return new(ApprovedStateEnum.ItemNotFound, null);

            var latestLog = await _logRoleUserRepository.GetLatestLogByParentId(id);
            if (latestLog.Activity != ActivityEnum.Submitted.ToString())
                return new(ApprovedStateEnum.HasBeenProcessed, null);

            if (state == ActivityEnum.Approved)
            {
                roleUser.ApprovedDate = DateTime.Now;
                roleUser.ApprovedBy = _currentUserService.CurrentUser?.UserName;
                await AssignDataAsync(latestLog, roleUser);
            }
            var logReference = latestLog.LgRoleUserRefs.ToList();
            if (state == ActivityEnum.Revised)
            {
                logReference = _mapper.Map<List<LgRoleUserRef>>(roleUser.MdRoleUserRefs);
            }
            InitilizeNewLogRererence(logReference);

            latestLog = _mapper.Map<MdRoleUser, LgRoleUser>(roleUser);
            latestLog.Id = 0;
            latestLog.ParentId = id;
            latestLog.Activity = state.ToString();
            latestLog.Note = note;
            latestLog.LgRoleUserRefs = logReference;
            await _logRoleUserRepository.AddAsync(latestLog);

            await _roleUserRepository.UpdateAsync(roleUser);
            await _taskRepository.DeleteByReferenceIdAsync(id, TaskListEnum.RoleUser);
            await _unitOfWorks.SaveChangesAsync();

            await _activityRepository.AddAsync(string.Format(TaskListMessage, $"User Role ({(await GetRole(roleUser.RoleId))?.Name})"));

            return new(ApprovedStateEnum.Success, roleUser.Id);
        }

        private static void InitilizeNewLogRererence(List<LgRoleUserRef> logReference)
        {
            logReference.ForEach(n => n.Id = 0);
            logReference.ForEach(n => n.ParentId = 0);
            logReference.ForEach(n => n.Parent = null);
        }

        private async Task AssignDataAsync(LgRoleUser latestLog, MdRoleUser roleUser)
        {
            roleUser.RoleId = latestLog.RoleId;
            roleUser.IsActive = latestLog.IsActive;
            foreach (var item in roleUser.MdRoleUserRefs)
            {
                _ = await _roleUserRefRepository.DeleteAsync(item, true);
            }

            foreach (var item in latestLog.LgRoleUserRefs)
            {
                var newReference = _mapper.Map<MdRoleUserRef>(item);
                newReference.ParentId = roleUser.Id;
                _ = await _roleUserRefRepository.AddAsync(newReference);
            }
            if (!string.IsNullOrEmpty(latestLog.DeletedBy))
            {
                roleUser.DeletedBy = latestLog.DeletedBy;
                roleUser.DeletedDate = latestLog.DeletedDate;
            }
        }

        private void AssignDeleteData(MdRoleUser roleUser, LgRoleUser logRoleUser)
        {
            roleUser.DeletedBy = logRoleUser.DeletedBy;
            roleUser.DeletedDate = logRoleUser.DeletedDate;
        }

        private async Task<Dictionary<string, object>> GenerateMetaDataAsync(MdRoleUser roleUser)
        {
            var dictionary = new Dictionary<string, object>();
            dictionary.Add("requestBy", _currentUserService.CurrentUser?.UserName);
            dictionary.Add("data", new { roleUser.Id, (await GetRole(roleUser.RoleId)).Name });
            return dictionary;
        }

        public async Task<Result<GetPagedUserRoleHistoryStateEnum, Paged<UserRoleHistoryDto>>> GePagedHistories(int id, PagedDto pagedDto)
        {
            var roleUser = await _roleUserRepository.GetById(id);
            if (roleUser == null)
                return new(GetPagedUserRoleHistoryStateEnum.ItemNotFound, null);
            return new(GetPagedUserRoleHistoryStateEnum.Success, _mapper.Map<Paged<UserRoleHistoryDto>>(await _logRoleUserRepository.GetPaged(id, _mapper.Map<PageFilter>(pagedDto))));
        }

        public async Task<Result<GetByIdUserRoleHistoryStateEnum, TmpRoleUserDto>> GetPreviewByIdAsync(int id)
        {
            var roleUser = await _roleUserRepository.GetById(id);
            if (roleUser == null)
                return new(GetByIdUserRoleHistoryStateEnum.ItemNotFound, null);

            var logRoleUser = await _logRoleUserRepository.GetLatestLogByParentId(id);
            var tempRoleUser = _mapper.Map<TmpRoleUserDto>(logRoleUser);
            tempRoleUser.Role = _mapper.Map<RoleDto>(await _roleRepository.GetById(logRoleUser.RoleId));
            return new(GetByIdUserRoleHistoryStateEnum.Success, tempRoleUser);
        }

        public Task<List<User>> GetAllApproverAsync()
        {
            return _roleUserRefRepository.GetAllApproverAsync();
        }

        public async Task<RoleUserDto> GetUserRoleByName(string roleName)
        {
            return _mapper.Map<RoleUserDto>(await _roleUserRepository.GetUserRoleByName(roleName));
        }
    }
}
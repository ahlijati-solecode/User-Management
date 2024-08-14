using Shared.Models;
using Shared.Models.Core;
using Shared.Models.Dtos.Core;
using User_Management.Models.Filters;
using static User_Management.Constants.RoleUserConstants;
using User_Management.Models.Dtos.UserRole;
using static Shared.Constants.ApiConstants;

namespace User_Management.Infrastructures.Services
{
    public interface IRoleUserService
    {
        Task<Result<CreateUniqueStateEnum, string>> CreateUniqueAsync(int? roleUserId,bool justView=false);
        Task<Result<AddRoleUserTemporaryStatEnum, string>> AddRoleUserAsync(string uniqueId, RoleUserDto RoleUserDto);
        Task<Result<UpdateRoleUserTemporaryStatEnum, string>> UpdateRoleUserAsync(string uniqueId, string id, RoleUserDto RoleUserDto);
        Task<Result<DeletedRoleUserTemporaryStatEnum, string>> DeleteRoleUserAsync(string uniqueId, string id);
        Task<List<User>> GetAllApproverAsync();
        Task<Paged<TmpRoleUserRefDto>> GetPagedRoleUserAsync(string uniqueId, PagedDto pagedDto, RoleUserFilter RoleUserFilter);
        Task<Result<SaveStatEnum, int?>> CommitRoleUserAsync(string uniqueId, RoleUserDto input);
        Task<Paged<LgRoleAccessDto>> GetPaged(PagedDto pagedDto, RoleUserFilter roleUserFilter);
        Task<Result<DeleteStateEnum, int?>> DeleteUserAsync(int id);
        Task<Result<ApprovedStateEnum, int?>> ApprovedAsync(int id, ActivityEnum state, string note);
        Task<Result<GetPagedUserRoleHistoryStateEnum, Paged<UserRoleHistoryDto>>> GePagedHistories(int id, PagedDto pagedDto);

        Task<Result<GetByIdUserRoleHistoryStateEnum, TmpRoleUserDto>> GetPreviewByIdAsync(int id);
        Task<RoleUserDto> GetUserRoleByName(string roleName);
    }
}
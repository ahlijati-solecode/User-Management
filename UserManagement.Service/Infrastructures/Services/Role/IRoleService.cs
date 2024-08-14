using Shared.Models;
using Shared.Models.Core;
using Shared.Models.Dtos.Core;
using User_Management.Models.Dtos.Role;
using User_Management.Models.Filters;
using static Shared.Constants.ApiConstants;
using static User_Management.Constants.RoleConstants;

namespace User_Management.Infrastructures.Services.Role
{
    public interface IRoleService
    {
        ActivityEnum CurrrentActivity { get; }

        Task<ApprovalStateEnum> ApprovalAsync(int id, RoleDto model, ActivityEnum state);

        Task<DeleteStateEnum> DeleteAsync(int id);

        Task<Result<GetByIdStateEnum, RoleDto>> GetAsync(int id);

        Task<Result<AddStateEnum, RoleDto?>> AddAsync(RoleDto model);

        Task<Result<UpdateStateEnum, RoleDto?>> UpdateAsync(int id, RoleDto model);

        Task<Paged<MdRolePagedDto>> GetPaged(PagedDto page, RoleFilter filter);

        Task<Result<GetRoleHistoiresEnum, Paged<LgRoleDto>?>> GePagedHistories(int roleId, PagedDto page);
        Task<Paged<MdRolePagedDto>> GetActiveAsync(PagedDto pagedDto, RoleFilter roleFilter);
        Task<RoleDto> GetActiveByNameAsync(string roleName);
    }
}
using Shared.Models.Core;
using Shared.Models.Dtos.Core;
using User_Management.Models.Dtos.User;
using User_Management.Models.Filters;
using static User_Management.Constants.RoleAccessConstant;

namespace User_Management.Infrastructures.Services
{
    public interface IRoleAccessService
    {
        Task<Tuple<AddStateEnum, RoleAccessDto>> AddAsync(RoleAccessDto input);
        Task<Tuple<UpdateStateEnum, RoleAccessDto>> UpdateAsync(int id, RoleAccessDto input);
        Task<DeleteStateEnum> DeleteAsync(int id);
        Task<Tuple<GetStateEnum, RoleAccessDto>> GetPreviewByIdAsync(int id);
        Task<Paged<RoleAccessDto>> GetPaged(PagedDto pagedDto, RoleAccessFilter userAccessFilter);

        Task<ApprovedStateEnum> ApprovedAsync(int id,string note);
        Task<RejectedStateEnum> RejectedAsync(int id, string note);
        Task<Tuple<GetRoleAccessHistoiresEnum, Paged<RoleAccessHistoryDto>?>> GePagedHistories(int id, PagedDto pagedDto);
        Task<IEnumerable<MenuDto>> GetMenu();
    }
}
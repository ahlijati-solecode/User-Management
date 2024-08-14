using Shared.Infrastructures.Repositories;
using Shared.Models.Core;
using User_Management.Models.Entities;
using User_Management.Models.Entities.Custom.RefUserAccess;
using User_Management.Models.Filters;

namespace User_Management.Infrastructures.Repositories.UserAccess
{
    public interface IRoleAccessRepository : IBaseRepository<MdRoleAccess>
    {
        Task<MdRoleAccess> GetPreviewByIdAsync(int id);
        Task<bool> IsDuplicateAsync(int roleId, int id);
        Task<Paged<MdUserAccessPaged>> GetPaged(RoleAccessFilter userAccessFilter, int page, int size, string sort);
    }
}

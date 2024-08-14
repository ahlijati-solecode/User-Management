using Shared.Infrastructures.Repositories;
using Shared.Models.Core;
using Shared.Models.Filters;
using User_Management.Models.Entities;
using User_Management.Models.Entities.Custom.Role;
using User_Management.Models.Filters;

namespace User_Management.Infrastructures.Repositories
{
    public interface IRoleRepository : IBaseRepository<MdRole>
    {
        Task<Paged<LgRolePaged>> GetPaged(RoleFilter filter, int page = 1, int size = 10, string sort = "id asc");
        Task<Paged<LgRolePaged>> GetPagedActive(RoleFilter roleFilter, PageFilter pageFilter);
        Task<MdRole> GetActiveById(int id);
        Task<bool> IsDuplicate(string name, int id);
        Task<MdRole> GetActiveByNameAsync(string roleName);
    }
}
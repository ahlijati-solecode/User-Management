using Shared.Infrastructures.Repositories;
using Shared.Models.Core;
using User_Management.Models.Entities;
using User_Management.Models.Entities.Custom.RoleUser;
using User_Management.Models.Filters;

namespace User_Management.Infrastructures.Repositories.RoleUser
{
    public interface IRoleUserRepository : IBaseRepository<MdRoleUser>
    {
        Task<Paged<RoleUserPaged>> GetPaged(RoleUserFilter emailFilter, int page, int size, string sort);
        Task<bool> IsDuplicate(int roleId, int id);
        Task<MdRoleUser> GetUserRoleByName(string roleName);
    }

}

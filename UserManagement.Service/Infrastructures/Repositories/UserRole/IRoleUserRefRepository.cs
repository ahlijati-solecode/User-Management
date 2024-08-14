using Shared.Infrastructures.Repositories;
using Shared.Models.Core;
using User_Management.Models.Entities;

namespace User_Management.Infrastructures.Repositories.RoleUser
{
    public interface IRoleUserRefRepository : IBaseRepository<MdRoleUserRef>
    {
        Task<List<MdRoleUserRef>> GetByUserName(string userName);
        Task<List<User>> GetAllApproverAsync();
        Task<List<MdRoleUserRef>> GetByRoles(int[] rolesId);

    }

}

using Shared.Infrastructures.Repositories;
using User_Management.Models.Entities;

namespace User_Management.Infrastructures.Repositories.UserAccess
{
    public interface IRoleAccessRefRepository : IBaseRepository<MdRoleAccessRef>
    {
        Task<List<MdRoleAccessRef>> GetByRoles(int[] rolesId);
    }
}

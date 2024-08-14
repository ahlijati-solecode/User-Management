using Shared.Infrastructures.Repositories;
using User_Management.Models.Entities;

namespace User_Management.Infrastructures.Repositories.RoleUser
{
    public interface ILogRoleUserRefRepository : IBaseRepository<LgRoleUserRef>
    {
        Task<List<LgRoleUserRef>> GetUsers(int id, int count);
    }

}

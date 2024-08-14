using Shared.Infrastructures.Repositories;
using Shared.Models.Core;
using Shared.Models.Filters;
using User_Management.Models.Entities;

namespace User_Management.Infrastructures.Repositories.RoleUser
{
    public interface ILogRoleUserRepository : IBaseRepository<LgRoleUser>
    {
        Task<LgRoleUser> GetLatestLogByParentId(int id);
        Task<Paged<LgRoleUser>> GetPaged(int id, PageFilter page);
    }

}

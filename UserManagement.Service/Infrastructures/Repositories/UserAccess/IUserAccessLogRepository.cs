using Shared.Infrastructures.Repositories;
using Shared.Models.Core;
using Shared.Models.Filters;
using User_Management.Models.Entities;

namespace User_Management.Infrastructures.Repositories.UserAccess
{
    public interface IRoleAccessLogRepository : IBaseRepository<LgRoleAccess>
    {
        Task<LgRoleAccess> GetLatestByUserAccessId(int id);
        Task<IEnumerable<LgRoleAccess>> GetLogs(int id);
        Task<Paged<LgRoleAccess>> GetPaged(int id, PageFilter filter);
    }
}

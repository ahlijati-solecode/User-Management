using Shared.Infrastructures.Repositories;
using User_Management.Models.Entities;

namespace User_Management.Infrastructures.Repositories.UserAccess
{
    public interface IRoleAccessRefLogRepository : IBaseRepository<LgRoleAccessRef>
    {
        Task<LgRoleAccessRef> GetLatestLog(int id);
        Task<IEnumerable<LgRoleAccessRef>> GetLogs(int id);
    }
}

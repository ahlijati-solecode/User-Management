using Shared.Infrastructures.Repositories.EntityFramework;
using User_Management.Models.Entities;

namespace User_Management.Infrastructures.Repositories.UserAccess
{
    public class EFUserAccessRefLogRepository : EFBaseRepository<LgRoleAccessRef>, IRoleAccessRefLogRepository
    {
        public EFUserAccessRefLogRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            IsAuditEntity = false;
        }

        public Task<LgRoleAccessRef> GetLatestLog(int id)
        {
            NoTraciking = true;
            return Task.FromResult(GetAll().Where(n => n.ParentId == id).OrderBy(n => n.Id).LastOrDefault());
        }

        public async Task<IEnumerable<LgRoleAccessRef>> GetLogs(int id)
        {
            NoTraciking = true;
            return GetAll().Where(n => n.ParentId == id).OrderBy(n => n.Id).ToList();
        }
    }
}
